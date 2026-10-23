using ShellX.ViewModels;

namespace ShellX.Pages;

public partial class BlackScholesPage : ContentPage
{
    // Below this width the two columns stack: chart on top, inputs and results scrolling underneath
    // (phones, or a narrowed desktop window).
    private const double PhoneWidth = 720;
    private const double PhoneChartHeight = 420;
    private const double PhoneMinColumnWidth = 64;

    private readonly BlackScholesViewModel _viewModel;
    private bool? _phone;

    public BlackScholesPage(BlackScholesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (width > 0)
            ApplyLayout(width < PhoneWidth);
    }

    // The views are never moved between parents (Android throws "child already has a parent");
    // the one grid is redefined and the two cards are repositioned within it.
    private void ApplyLayout(bool phone)
    {
        if (_phone == phone)
            return;
        _phone = phone;

        if (phone)
        {
            WideLayout.ColumnDefinitions = [new ColumnDefinition(GridLength.Star)];
            WideLayout.RowDefinitions = [new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star)];
            WideLayout.Padding = new Thickness(8);
            WideLayout.RowSpacing = 8;
            WideLayout.ColumnSpacing = 0;
            Place(ChartCard, row: 0, column: 0);
            Place(LeftScroll, row: 1, column: 0);
            ChartCard.HeightRequest = PhoneChartHeight;
        }
        else
        {
            WideLayout.ColumnDefinitions = [new ColumnDefinition(new GridLength(2, GridUnitType.Star)), new ColumnDefinition(new GridLength(3, GridUnitType.Star))];
            WideLayout.RowDefinitions = [new RowDefinition(GridLength.Star)];
            WideLayout.Padding = new Thickness(10);
            WideLayout.RowSpacing = 0;
            WideLayout.ColumnSpacing = 10;
            Place(LeftScroll, row: 0, column: 0);
            Place(ChartCard, row: 0, column: 1);
            ChartCard.HeightRequest = -1;
        }

        var minColumnWidth = phone ? PhoneMinColumnWidth : 0;
        ResultsGrid.MinColumnWidth = minColumnWidth;
        ResultsGrid2.MinColumnWidth = minColumnWidth;
    }

    private static void Place(BindableObject view, int row, int column)
    {
        Grid.SetRow(view, row);
        Grid.SetColumn(view, column);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.ActivateAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _viewModel.DeactivateAsync();
    }
}
