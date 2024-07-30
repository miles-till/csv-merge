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
    new("--working-directory", () => null, "The working directory to search for files in.");
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
    if (string.IsNullOrWhiteSpace(workingDirectoryPath))
    {
        workingDirectoryPath = Environment.CurrentDirectory;
    }

    Matcher matcher = new();
    matcher.AddIncludePatterns(searchPatterns);
    string[] files = matcher.GetResultsInFullPath(workingDirectoryPath).ToArray();

    Console.WriteLine($"Merging {files.Length} files...");

    FileInfo output = new(outputPath);
    MergeOptions mergeOptions = new() { Output = output, FilepathField = filepathField };
    await Merger.Merge(files, mergeOptions);

    Console.WriteLine($"Merge output: {output.FullName}");
}
