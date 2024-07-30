using System.CommandLine;
using CsvMerge;
using Microsoft.Extensions.FileSystemGlobbing;

Argument<string[]> searchPatternsArgument =
    new("search-patterns", "Glob search patterns for the csv files to merge.")
    {
        Arity = ArgumentArity.OneOrMore,
    };
Option<string> outputOption = new("--output", "The output result of merging csv files.");
Option<string> filepathFieldOption =
    new(
        "--filepath-field",
        () => "Filepath",
        "The header for the filepath field included in the merged csv file.)"
    );
Option<string?> workingDirectoryOption =
    new("--working-directory", "The working directory to search for files in.")
    {
        Arity = ArgumentArity.ZeroOrOne,
    };
Option<bool> overwriteOutput =
    new("--overwrite", () => true, "Overwrite output file if it already exists.");
RootCommand rootCommand = new("CsvMerge");
rootCommand.AddArgument(searchPatternsArgument);
rootCommand.AddOption(outputOption);
rootCommand.AddOption(filepathFieldOption);
rootCommand.AddOption(workingDirectoryOption);
rootCommand.AddOption(overwriteOutput);
rootCommand.SetHandler(
    Process,
    searchPatternsArgument,
    outputOption,
    filepathFieldOption,
    workingDirectoryOption,
    overwriteOutput
);
await rootCommand.InvokeAsync(args);
return;

static async Task Process(
    string[] searchPatterns,
    string outputPath,
    string filepathField,
    string? workingDirectoryPath,
    bool overwrite
)
{
    Console.WriteLine($"<search-patterns> arg = {searchPatterns}");
    Console.WriteLine($"<output> option = {outputPath}");
    Console.WriteLine($"<working-directory> option = {workingDirectoryPath}");
    Console.WriteLine($"<filepath-field> option = {filepathField}");
    Console.WriteLine($"<overwrite> option = {overwrite}");

    if (string.IsNullOrWhiteSpace(workingDirectoryPath))
    {
        workingDirectoryPath = Environment.CurrentDirectory;
    }

    Matcher matcher = new();
    matcher.AddIncludePatterns(searchPatterns);
    var files = matcher.GetResultsInFullPath(workingDirectoryPath);

    FileInfo output = new(outputPath);

    MergeOptions mergeOptions = new() { Output = output, FilepathField = filepathField };
    await Merger.Merge(files, mergeOptions);
}
