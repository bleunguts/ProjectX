namespace ShellX.Services;

public interface IDialogService
{
    Task AlertAsync(string title, string message);
}

public class DialogService : IDialogService
{
    public async Task AlertAsync(string title, string message)
    {
        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page is not null)
            await MainThread.InvokeOnMainThreadAsync(() => page.DisplayAlertAsync(title, message, "OK"));
    }
}
