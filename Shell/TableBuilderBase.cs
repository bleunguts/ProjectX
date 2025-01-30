using System.Data;

namespace Shell;

public abstract class TableBuilderBase
{
    protected readonly DataTable _dt = new DataTable();

    protected abstract DataColumn[] Headers { get; }

    public TableBuilderBase()
    {
        _dt.Columns.AddRange(Headers);
    }
    public abstract DataTable Build();
}

