using ShellX.ViewModels;

namespace ShellX.Pages;

public partial class BlackScholesPage : ContentPage
{
    private readonly BlackScholesViewModel _viewModel;

    public BlackScholesPage(BlackScholesViewModel viewModel)
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
