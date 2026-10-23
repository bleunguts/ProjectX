namespace ShellX.Tables;

public abstract class TableBuilderBase
{
    protected readonly List<string[]> _rows = new();

    protected abstract string[] Headers { get; }

    public abstract TableData Build();

    protected TableData ToTable(IEnumerable<string[]> rows) => new(Headers, rows.ToList());
}
