using System.ComponentModel;
using ShellX.ViewModels;

namespace ShellX.Pages;

/// <summary>The showcase version of the Options screen: animated shader backdrop, full-bleed rotating surface, glass cards and Greek dials.</summary>
public partial class BlackScholesHeroPage : ContentPage
{
    // Below this width (phones, or a narrowed window) the cards stack under the hero area.
    private const double PhoneWidth = 720;
    private const double PhoneHeroHeight = 560;

    private readonly BlackScholesViewModel _viewModel;
    private bool? _phone;

    public BlackScholesHeroPage(BlackScholesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (width <= 0 || height <= 0)
            return;

        ApplyLayout(width < PhoneWidth);

        // Wide: the layout is exactly as tall as the window so the right column can stretch. Phone: it grows and scrolls.
        var wanted = _phone == true ? -1 : height;
        if (Math.Abs(Layout.HeightRequest - wanted) > 0.5)
            Layout.HeightRequest = wanted;
    }

    // Views are repositioned, never re-parented (Android throws "child already has a parent").
    private void ApplyLayout(bool phone)
    {
        if (_phone == phone)
            return;
        _phone = phone;

        if (phone)
        {
            Layout.ColumnDefinitions = [new ColumnDefinition(GridLength.Star)];
            Layout.RowDefinitions = [new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto)];
            Layout.Padding = new Thickness(10);
            Layout.RowSpacing = 12;
            Layout.ColumnSpacing = 0;
            Place(RightColumn, row: 0, column: 0);
            Place(LeftScroll, row: 1, column: 0);
            RightColumn.HeightRequest = PhoneHeroHeight;

            // The surface stays pinned behind the top of the page while the cards scroll over it.
            HeroSurface.VerticalOptions = LayoutOptions.Start;
            HeroSurface.HeightRequest = PhoneHeroHeight - 190;
            HeroSurface.Margin = new Thickness(0, 44, 0, 0);
            Foreground.InputTransparent = false;
        }
        else
        {
            Layout.ColumnDefinitions = [new ColumnDefinition(new GridLength(380)), new ColumnDefinition(GridLength.Star)];
            Layout.RowDefinitions = [new RowDefinition(GridLength.Star)];
            Layout.Padding = new Thickness(14);
            Layout.RowSpacing = 0;
            Layout.ColumnSpacing = 14;
            Place(LeftScroll, row: 0, column: 0);
            Place(RightColumn, row: 0, column: 1);
            RightColumn.HeightRequest = -1;

            // Fill the clear area: right of the cards, below the chips, above the dials.
            HeroSurface.VerticalOptions = LayoutOptions.Fill;
            HeroSurface.HeightRequest = -1;
            HeroSurface.Margin = new Thickness(408, 52, 14, 190);

            // Let drags in the clear area reach the surface; the cards and dials still take their own input.
            Foreground.InputTransparent = true;
            Foreground.CascadeInputTransparent = false;
        }
    }

    private static void Place(BindableObject view, int row, int column)
    {
        Grid.SetRow(view, row);
        Grid.SetColumn(view, column);
    }

    // New results: a quick pulse on the price card.
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(BlackScholesViewModel.ResultsVersion))
            return;

        // Open on the middle maturity: at the shortest one most Greeks sit at the low end of their dials.
        _viewModel.SelectMaturityCommand.Execute((int)Math.Round(_viewModel.MaxMaturityIndex / 2));
        _ = PulseHeroAsync();
    }

    private async Task PulseHeroAsync()
    {
        HeroCard.AbortAnimation("scale");
        await HeroCard.ScaleToAsync(1.03, 120);
        await HeroCard.ScaleToAsync(1, 180);
    }

    private void OnCalculatorChipClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: string key })
            _viewModel.SelectCalculatorCommand.Execute(key);
    }

    private void OnPlotChipClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: string key })
            return;

        if (key == "Experimental")
            _viewModel.PlotExperimentalCommand.Execute(null);
        else
            _viewModel.PlotCommand.Execute(key);
    }

    private async void OnTileTapped(object? sender, TappedEventArgs e)
    {
        if (ViewOf(sender) is not { } tile)
            return;

        await tile.ScaleToAsync(0.97, 60);
        await tile.ScaleToAsync(1, 90);
    }

    private async void OnTilePointerEntered(object? sender, PointerEventArgs e)
    {
        if (ViewOf(sender) is { } tile)
            await tile.ScaleToAsync(1.02, 90);
    }

    private async void OnTilePointerExited(object? sender, PointerEventArgs e)
    {
        if (ViewOf(sender) is { } tile)
            await tile.ScaleToAsync(1, 90);
    }

    private static VisualElement? ViewOf(object? sender) => sender as VisualElement ?? (sender as Element)?.Parent as VisualElement;

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
