using ShellX.ViewModels;

namespace ShellX.Pages;

public partial class FxPricerPage : ContentPage
{
    private readonly FxPricerViewModel _viewModel;

    public FxPricerPage(FxPricerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
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
