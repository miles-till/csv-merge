namespace CsvMerge;

public static class Merger
{
    public static Task Merge(IEnumerable<string> files, MergeOptions options)
    {
        return Merge(files.Select(o => new FileInfo(o)), options);
    }

    public static async Task Merge(IEnumerable<FileInfo> files, MergeOptions options)
    {
        var fileInfos = files as FileInfo[] ?? files.ToArray();

        // validate
        var output = options.Output;
        if (!options.Overwrite && output.Exists)
            throw new Exception("Output file already exists.");

        if (fileInfos.Length == 0)
            throw new Exception("No files to merge.");

        foreach (var file in fileInfos)
        {
            if (!file.Exists)
                throw new FileNotFoundException(file.FullName);
        }

        string? headers = await Helpers.GetHeadersAsync(fileInfos[0]);
        if (string.IsNullOrWhiteSpace(headers))
            throw new Exception("Unable to read headers.");

        // merge files
        FileInfo tempFile = new(Path.GetTempFileName());
        try
        {
            await using (var tempFileStream = tempFile.OpenWrite())
            await using (StreamWriter tempWriter = new(tempFileStream))
            {
                if (!string.IsNullOrWhiteSpace(options.FilepathField))
                {
                    await tempWriter.WriteAsync(options.FilepathField + ",");
                }

                await tempWriter.WriteLineAsync(headers);

                foreach (var file in fileInfos)
                {
                    await using var fileStream = file.OpenRead();
                    using StreamReader fileReader = new(fileStream);

                    string? fileHeaders = await fileReader.ReadLineAsync();
                    if (fileHeaders != headers)
                        throw new Exception("Headers don't match.");

                    while (!fileReader.EndOfStream)
                    {
                        string? line = await fileReader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        if (!string.IsNullOrWhiteSpace(options.FilepathField))
                        {
                            await tempWriter.WriteAsync(file.FullName + ",");
                        }

                        await tempWriter.WriteLineAsync(line);
                    }
                }
            }

            tempFile.CopyTo(options.Output.FullName, options.Overwrite);
        }
        finally
        {
            tempFile.Delete();
        }
    }
}
