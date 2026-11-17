using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShellX.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            // WinUI errors such as "Element not found" arrive without a useful stack in the debugger;
            // write them to a file (shown in the debug output too) so the real cause can be read.
            UnhandledException += (_, e) => LogCrash("WinUI", e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, e) => LogCrash("AppDomain", e.ExceptionObject as Exception);
            TaskScheduler.UnobservedTaskException += (_, e) => LogCrash("Task", e.Exception);
        }

        private static void LogCrash(string source, Exception? exception)
        {
            try
            {
                var folder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
                var entry = $"[{DateTime.Now:O}] {source}{Environment.NewLine}{exception}{Environment.NewLine}{Environment.NewLine}";
                File.AppendAllText(Path.Combine(folder, "ShellX-crash.log"), entry);
                System.Diagnostics.Debug.WriteLine(entry);
            }
            catch
            {
                // Logging must never hide the original failure.
            }
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
