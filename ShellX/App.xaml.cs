namespace ShellX
{
    public partial class App : Application
    {
        private readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Light;
            _services = services;
        }

        // The shell is resolved here, not injected into the constructor: its XAML uses {StaticResource ...}
        // from App.xaml, which only exists once InitializeComponent above has loaded the resource dictionaries.
        protected override Window CreateWindow(IActivationState? activationState) => new(_services.GetRequiredService<AppShell>());
    }
}
