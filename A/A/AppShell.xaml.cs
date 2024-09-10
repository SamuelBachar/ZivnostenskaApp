using A.Views;
using A.Views.LogIn;
using A.Views.Register;

namespace A
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LogInChooseView), typeof(LogInChooseView));
            Routing.RegisterRoute(nameof(RegisterCompanyView), typeof(RegisterCompanyView));
            Routing.RegisterRoute(nameof(RegisterChooseView), typeof(RegisterChooseView));
        }
    }
}
