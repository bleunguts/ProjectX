using ShellX.Services;

namespace ShellX
{
    public partial class AppShell : Shell
    {
        public AppShell(StatusService status)
        {
            InitializeComponent();
            BindingContext = status;
        }
    }
}
