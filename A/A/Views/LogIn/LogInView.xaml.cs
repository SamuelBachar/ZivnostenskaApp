using A.Exceptions.UserActionException;
using A.Interfaces;
using SharedTypesLibrary.DTOs.Response;
using static A.Enums.Enums;

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
        App.AppMode = AppMode.Customer;
        await LoginGeneric(EntryEmail.Text, EntryPassword.Text);
    }

    private async void BtnSignInAsCompany_Clicked(object sender, EventArgs e)
    {
        App.AppMode = AppMode.Company;
        await LoginGeneric(EntryEmail.Text, EntryPassword.Text);
    }

    private async Task LoginGeneric(string email, string password)
    {
        (UserLoginDataDTO UserLoginDTO, ExceptionHandler Message) response = await _loginService.LoginHTTPS(EntryEmail.Text, EntryPassword.Text);

        if (response.UserLoginDTO != null)
        {
            App.UserData.JWT = response.UserLoginDTO.JWT;
            // navigate to main view of certian AppMode
        }
    }
}