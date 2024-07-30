namespace CsvMerge;

internal static class Helpers
{
    internal static async Task<string?> GetHeadersAsync(FileInfo file)
    {
        await using var fileStream = file.OpenRead();
        using StreamReader reader = new(fileStream);
        return await reader.ReadLineAsync();
    }
}
