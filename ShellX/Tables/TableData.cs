namespace ShellX.Tables;

public record TableData(IReadOnlyList<string> Headers, IReadOnlyList<string[]> Rows);
