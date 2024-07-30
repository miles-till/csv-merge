namespace CsvMerge.Core.Tests;

using FluentAssertions;

public class HelperTests
{
    public sealed class GetHeadersTests
    {
        [Fact]
        public async Task GetsHeadersLineFromFile()
        {
            FileInfo file = new("Samples/file1.csv");
            string? headers = await Helpers.GetHeadersAsync(file);

            string[] fileLines = await File.ReadAllLinesAsync(file.FullName);
            headers.Should().NotBeNull();
            headers.Should().Be(fileLines[0]);
        }
    }
}
