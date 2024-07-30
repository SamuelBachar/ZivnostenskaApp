using A.Interfaces;
using SharedTypesLibrary.DTOs.Response;

namespace A.Views;

public partial class LogInView : ContentPage
{
    readonly ILoginService _loginService = null;
    public LogInView(ILoginService loginService)
	{
		InitializeComponent();

        _loginService = loginService;
	}

    private void EntryEmail_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void EntryPassword_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private async void BtnSignInAsUser_Clicked(object sender, EventArgs e)
    {
        (UserLoginDataDTO UserLoginDTO, string Message) response = await _loginService.LoginHTTPS(EntryEmail.Text, EntryPassword.Text);
    }

    private async void BtnSignInAsCompany_Clicked(object sender, EventArgs e)
    {

    }
}