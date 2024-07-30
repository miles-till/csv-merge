namespace CsvMerge;

public class MergeOptions
{
    public required FileInfo Output { get; init; }
    public string FilepathField { get; init; } = "Filepath";
    public bool Overwrite { get; init; } = true;
}
