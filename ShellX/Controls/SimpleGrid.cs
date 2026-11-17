using ShellX.Tables;

namespace ShellX.Controls;

/// <summary>A read-only table: a header row over a virtualised list of rows, with optional single-row selection.</summary>
public class SimpleGrid : ContentView
{
    private const double CellFontSize = 11;
    private static readonly Thickness CellPadding = new(3, 1);

    public static readonly BindableProperty TableProperty = BindableProperty.Create(
        nameof(Table), typeof(TableData), typeof(SimpleGrid), null,
        propertyChanged: (bindable, _, _) => ((SimpleGrid)bindable).Rebuild());

    public static readonly BindableProperty SelectedRowProperty = BindableProperty.Create(
        nameof(SelectedRow), typeof(string[]), typeof(SimpleGrid), null, BindingMode.TwoWay);

    public static readonly BindableProperty BodyHeightProperty = BindableProperty.Create(
        nameof(BodyHeight), typeof(double), typeof(SimpleGrid), 180d,
        propertyChanged: (bindable, _, _) => ((SimpleGrid)bindable).Rebuild());

    public static readonly BindableProperty SignColorColumnProperty = BindableProperty.Create(
        nameof(SignColorColumn), typeof(string), typeof(SimpleGrid), null,
        propertyChanged: (bindable, _, _) => ((SimpleGrid)bindable).Rebuild());

    public static readonly BindableProperty MinColumnWidthProperty = BindableProperty.Create(
        nameof(MinColumnWidth), typeof(double), typeof(SimpleGrid), 0d,
        propertyChanged: (bindable, _, _) => ((SimpleGrid)bindable).Rebuild());

    public TableData? Table
    {
        get => (TableData?)GetValue(TableProperty);
        set => SetValue(TableProperty, value);
    }

    public string[]? SelectedRow
    {
        get => (string[]?)GetValue(SelectedRowProperty);
        set => SetValue(SelectedRowProperty, value);
    }

    /// <summary>Height of the scrolling body, excluding the header row.</summary>
    public double BodyHeight
    {
        get => (double)GetValue(BodyHeightProperty);
        set => SetValue(BodyHeightProperty, value);
    }

    /// <summary>When above zero each column is at least this wide and the grid scrolls sideways if the space is narrower, instead of truncating cell text.</summary>
    public double MinColumnWidth
    {
        get => (double)GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
    }

    /// <summary>Header of a column whose cells turn green when positive and red when negative (for PnL).</summary>
    public string? SignColorColumn
    {
        get => (string?)GetValue(SignColorColumnProperty);
        set => SetValue(SignColorColumnProperty, value);
    }

    public event EventHandler<string[]>? RowSelected;

    private void Rebuild()
    {
        if (Table is not { } table)
        {
            Content = null;
            return;
        }

        var columns = table.Headers.Count;

        var header = NewRow(columns);
        for (int i = 0; i < columns; i++)
            header.Add(new Label { Text = table.Headers[i], FontAttributes = FontAttributes.Bold, FontSize = CellFontSize, Padding = CellPadding }, i, 0);

        var body = new CollectionView
        {
            ItemsSource = table.Rows,
            SelectionMode = SelectionMode.Single,
            HeightRequest = BodyHeight,
            ItemTemplate = new DataTemplate(() =>
            {
                var row = NewRow(columns);
                for (int i = 0; i < columns; i++)
                {
                    var cell = new Label { FontSize = CellFontSize, Padding = CellPadding, LineBreakMode = LineBreakMode.TailTruncation };
                    cell.SetBinding(Label.TextProperty, $"[{i}]");
                    if (table.Headers[i] == SignColorColumn)
                        cell.SetBinding(Label.TextColorProperty, $"[{i}]", converter: SignColorConverter.Instance);
                    row.Add(cell, i, 0);
                }
                return row;
            }),
        };
        body.SelectionChanged += (_, e) =>
        {
            if (e.CurrentSelection.FirstOrDefault() is string[] selected)
            {
                SelectedRow = selected;
                RowSelected?.Invoke(this, selected);
            }
        };

        var layout = new Grid { RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto) } };
        layout.Add(header, 0, 0);
        layout.Add(body, 0, 1);
        if (MinColumnWidth > 0)
        {
            layout.WidthRequest = columns * MinColumnWidth;
            Content = new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = layout };
        }
        else
        {
            Content = layout;
        }
    }

    private static Grid NewRow(int columns)
    {
        var row = new Grid();
        for (int i = 0; i < columns; i++)
            row.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        return row;
    }
}
