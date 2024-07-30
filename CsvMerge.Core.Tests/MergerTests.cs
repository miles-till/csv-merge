namespace CsvMerge.Core.Tests;

using FluentAssertions;

public class MergerTests
{
    public sealed class ZeroFiles
    {
        private readonly MergeOptions _options;

        public ZeroFiles()
        {
            FileInfo tempFile = new(Path.GetTempFileName());
            _options = new MergeOptions { Output = tempFile };
        }

        [Fact]
        public void ShouldThrow()
        {
            FileInfo[] files = [];
            Func<Task> a = async () => await Merger.Merge(files, _options);
            a.Should().ThrowAsync<Exception>();
        }
    }

    public sealed class SingleFile : IDisposable
    {
        private readonly MergeOptions _options;

        public SingleFile()
        {
            FileInfo tempFile = new(Path.GetTempFileName());
            _options = new MergeOptions { Output = tempFile };
        }

        public void Dispose()
        {
            _options.Output.Delete();
        }

        [Fact]
        public async Task ShouldMerge()
        {
            FileInfo[] files = [new FileInfo("./Samples/file1.csv")];
            await Merger.Merge(files, _options);

            string[] fileLines = await File.ReadAllLinesAsync(files[0].FullName);
            var fileLinesWithFilepath = fileLines.Select(
                (o, i) => (i == 0 ? "Filepath," : files[0].FullName + ",") + o
            );
            string[] mergedLines = await File.ReadAllLinesAsync(_options.Output.FullName);
            mergedLines.Should().BeEquivalentTo(fileLinesWithFilepath);
        }
    }

    public sealed class MultiFile : IDisposable
    {
        private readonly MergeOptions _options;

        public MultiFile()
        {
            FileInfo tempFile = new(Path.GetTempFileName());
            _options = new MergeOptions { Output = tempFile };
        }

        public void Dispose()
        {
            _options.Output.Delete();
        }

        [Fact]
        public async Task ShouldMerge()
        {
            FileInfo[] files =
            [
                new FileInfo("./Samples/file1.csv"),
                new FileInfo("./Samples/file2.csv"),
                new FileInfo("./Samples/file3.csv"),
            ];
            await Merger.Merge(files, _options);

            string[] file1Lines = await File.ReadAllLinesAsync(files[0].FullName);
            string[] file2Lines = await File.ReadAllLinesAsync(files[1].FullName);
            string[] file3Lines = await File.ReadAllLinesAsync(files[2].FullName);
            var fileLinesWithFilepath = file1Lines
                .Select((o, i) => (i == 0 ? "Filepath," : files[0].FullName + ",") + o)
                .Concat(file2Lines.Skip(1).Select(o => files[1].FullName + "," + o))
                .Concat(file3Lines.Skip(1).Select(o => files[2].FullName + "," + o));
            string[] mergedLines = await File.ReadAllLinesAsync(_options.Output.FullName);
            mergedLines.Should().BeEquivalentTo(fileLinesWithFilepath);
        }
    }
}
