using ShellX.ViewModels;

namespace ShellX.Pages;

public partial class BlackScholesPage : ContentPage
{
    // Below this width the two columns stack into one scrolling column (phones, or a narrowed desktop window).
    private const double PhoneWidth = 720;
    private const double PhoneChartHeight = 380;
    private const double PhoneMinColumnWidth = 64;

    private readonly BlackScholesViewModel _viewModel;
    private readonly VerticalStackLayout _phoneStack = new() { Spacing = 10, Padding = new Thickness(8) };
    private ScrollView? _phoneScroll;
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

    private void ApplyLayout(bool phone)
    {
        if (_phone == phone)
            return;
        _phone = phone;

        if (phone)
        {
            // Chart on top at a fixed height, then the inputs and results, all in one vertical scroll.
            WideLayout.Remove(ChartCard);
            LeftScroll.Content = null;
            ChartCard.HeightRequest = PhoneChartHeight;
            _phoneStack.Add(ChartCard);
            _phoneStack.Add(LeftStack);
            _phoneScroll ??= new ScrollView { Content = _phoneStack };
            Content = _phoneScroll;
        }
        else
        {
            _phoneStack.Remove(ChartCard);
            _phoneStack.Remove(LeftStack);
            ChartCard.HeightRequest = -1;
            LeftScroll.Content = LeftStack;
            WideLayout.Add(ChartCard);
            Content = WideLayout;
        }

        var minColumnWidth = phone ? PhoneMinColumnWidth : 0;
        ResultsGrid.MinColumnWidth = minColumnWidth;
        ResultsGrid2.MinColumnWidth = minColumnWidth;
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
