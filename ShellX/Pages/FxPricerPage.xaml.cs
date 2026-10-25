using System.ComponentModel;
using ShellX.Fx;
using ShellX.ViewModels;

namespace ShellX.Pages;

public partial class FxPricerPage : ContentPage
{
    private const string FlashAnimation = "flash";

    private static readonly Color UpColor = Color.FromArgb("#16A34A");
    private static readonly Color DownColor = Color.FromArgb("#DC2626");

    private readonly FxPricerViewModel _viewModel;
    private readonly IDispatcherTimer _liveTimer;

    public FxPricerPage(FxPricerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _liveTimer = Dispatcher.CreateTimer();
        _liveTimer.Interval = TimeSpan.FromSeconds(1);
        _liveTimer.Tick += (_, _) => _viewModel.RefreshLiveness();
    }

    // Below this width (phones, or a narrowed window) the two columns stack into one scrolling column.
    private const double PhoneWidth = 720;

    private bool? _phone;

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (width <= 0 || height <= 0)
            return;

        ApplyLayout(width < PhoneWidth);

        // Wide: the grid is exactly as tall as the window so the history and tail rows can stretch.
        // Phone: let it grow with its content and scroll.
        var wanted = _phone == true ? -1 : height;
        if (Math.Abs(WideLayout.HeightRequest - wanted) > 0.5)
            WideLayout.HeightRequest = wanted;
    }

    // The views are never moved between parents (Android throws "child already has a parent"):
    // the one grid is redefined and the two halves are repositioned within it.
    private void ApplyLayout(bool phone)
    {
        if (_phone == phone)
            return;
        _phone = phone;

        if (phone)
        {
            WideLayout.ColumnDefinitions = [new ColumnDefinition(GridLength.Star)];
            WideLayout.RowDefinitions = [new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto)];
            WideLayout.Padding = new Thickness(8);
            WideLayout.RowSpacing = 10;
            WideLayout.ColumnSpacing = 0;
            Place(LeftScroll, row: 0, column: 0);
            Place(RightGrid, row: 1, column: 0);

            RightGrid.RowDefinitions = [.. Enumerable.Range(0, 5).Select(_ => new RowDefinition(GridLength.Auto))];
            HistoryCard.HeightRequest = 240;
            TailCard.HeightRequest = 150;
            ChartRow.ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(new GridLength(120))];

            // Subscribe/Unsubscribe get their own full-width row so they are never pushed off the edge.
            Grid.SetRow(SubscribeButtons, 2);
            Grid.SetColumn(SubscribeButtons, 0);
            Grid.SetColumnSpan(SubscribeButtons, 3);
        }
        else
        {
            WideLayout.ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star)];
            WideLayout.RowDefinitions = [new RowDefinition(GridLength.Star)];
            WideLayout.Padding = new Thickness(10);
            WideLayout.RowSpacing = 0;
            WideLayout.ColumnSpacing = 10;
            Place(LeftScroll, row: 0, column: 0);
            Place(RightGrid, row: 0, column: 1);

            RightGrid.RowDefinitions =
            [
                new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto),
                new RowDefinition(new GridLength(2, GridUnitType.Star)), new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto),
            ];
            HistoryCard.HeightRequest = -1;
            TailCard.HeightRequest = -1;
            ChartRow.ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(new GridLength(200))];

            Grid.SetRow(SubscribeButtons, 1);
            Grid.SetColumn(SubscribeButtons, 2);
            Grid.SetColumnSpan(SubscribeButtons, 1);
        }

        // The last-tick bar is one line: on a phone drop the timestamp and shrink the price so they do not overlap.
        LastTickPrice.FontSize = phone ? 15 : 20;
        LastTickTime.IsVisible = !phone;

        // Smaller price figures so a half-width tile on a phone is not clipped.
        SetTileFonts(phone ? (18, 44, 22) : (26, 64, 32));
    }

    private void SetTileFonts((double Prefix, double Big, double Fraction) sizes)
    {
        foreach (var label in new[] { SellPrefix, BuyPrefix }) label.FontSize = sizes.Prefix;
        foreach (var label in new[] { SellBig, BuyBig }) label.FontSize = sizes.Big;
        foreach (var label in new[] { SellFraction, BuyFraction }) label.FontSize = sizes.Fraction;
    }

    private static void Place(BindableObject view, int row, int column)
    {
        Grid.SetRow(view, row);
        Grid.SetColumn(view, column);
    }

    // One flash per tick: TickCount changes every tick, TickDirection only when the direction changes.
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FxPricerViewModel.TickCount))
            Flash(_viewModel.TickDirection);
    }

    private void Flash(FxTickDirection direction)
    {
        if (direction == FxTickDirection.Flat)
            return;

        var tint = direction == FxTickDirection.Up ? UpColor : DownColor;
        PriceFlash.AbortAnimation(FlashAnimation);
        new Animation(t => PriceFlash.BackgroundColor = tint.WithAlpha((float)(0.35 * (1 - t))), 0, 1)
            .Commit(PriceFlash, FlashAnimation, length: 350, easing: Easing.CubicOut,
                    finished: (_, _) => PriceFlash.BackgroundColor = Colors.Transparent);
    }

    // The price tiles are buttons: lift slightly under the pointer, dip when pressed.
    private async void OnTileTapped(object? sender, TappedEventArgs e)
    {
        if (!_viewModel.HasPrice || ViewOf(sender) is not { } tile)
            return;

        await tile.ScaleToAsync(0.96, 60);
        await tile.ScaleToAsync(1, 90);
    }

    private async void OnTilePointerEntered(object? sender, PointerEventArgs e)
    {
        if (_viewModel.HasPrice && ViewOf(sender) is { } tile)
            await tile.ScaleToAsync(1.02, 90);
    }

    private async void OnTilePointerExited(object? sender, PointerEventArgs e)
    {
        if (ViewOf(sender) is { } tile)
            await tile.ScaleToAsync(1, 90);
    }

    private static VisualElement? ViewOf(object? sender) => sender as VisualElement ?? (sender as Element)?.Parent as VisualElement;

    private void OnQuantityChipClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: int quantity })
            _viewModel.SetQuantityCommand.Execute(quantity);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _liveTimer.Start();
        await _viewModel.ActivateAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        _liveTimer.Stop();
        await _viewModel.DeactivateAsync();
    }
}
