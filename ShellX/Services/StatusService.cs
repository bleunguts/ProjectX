using CommunityToolkit.Mvvm.ComponentModel;

namespace ShellX.Services;

public partial class StatusService : ObservableObject
{
    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private double _progress;

    [ObservableProperty]
    private bool _isBusy;

    public void Report(string text) => MainThread.BeginInvokeOnMainThread(() => StatusText = text);

    public void Report(string text, double progress)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            StatusText = text;
            Progress = progress;
            IsBusy = progress is > 0 and < 1;
        });
    }
}
