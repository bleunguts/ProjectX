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
