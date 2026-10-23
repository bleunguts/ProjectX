namespace ShellX
{
    public partial class App : Application
    {
        private readonly AppShell _shell;

        public App(AppShell shell)
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Light;
            _shell = shell;
        }

        protected override Window CreateWindow(IActivationState? activationState) => new(_shell);
    }
}
