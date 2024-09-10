using A.Views;
using A.Views.LogIn;

namespace A
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LogInChooseView), typeof(LogInChooseView));
            Routing.RegisterRoute(nameof(RegisterCompanyView), typeof(RegisterCompanyView));
        }
    }
}
