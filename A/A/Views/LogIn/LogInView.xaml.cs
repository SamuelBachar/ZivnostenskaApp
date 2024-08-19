using A.AppPreferences;
using A.Interfaces;
using A.Services;
using ExceptionsHandling;
using Newtonsoft.Json;
using SharedTypesLibrary.Constants;
using SharedTypesLibrary.DTOs.Response;
using System;
using static A.Enums.Enums;

namespace A.Views;

public partial class LogInView : ContentPage
{
    readonly ILoginService _loginService = null;
    WebView _webView = null;

    public LogInView(ILoginService loginService)
	{
		InitializeComponent();

        _loginService = loginService;
	}

    private async void ChkRememberLogin_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            await SettingsService.SaveStaticAsync<bool>(PrefUserSettings.PrefRememberLogIn, true);
        }
        else
        {
            await SettingsService.SaveStaticAsync<bool>(PrefUserSettings.PrefRememberLogIn, false);
        }
    }

    private async void txtForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        //await Shell.Current.GoToAsync(nameof(ForgotPasswordView));
    }

    private async void TxtRegisterHere_Tapped(object sender, TappedEventArgs e)
    {
        //await Shell.Current.GoToAsync(nameof(RegisterView));
    }

    private void EntryPassword_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!EntryPassword.IsPassword && EntryPassword.PlaceholderColor == Colors.OrangeRed)
        {
            EntryPassword.IsPassword = true;
            EntryPassword.TextColor = Colors.Black;
        }
    }

    private void EntryEmail_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (EntryEmail.PlaceholderColor == Colors.OrangeRed)
        {
            EntryEmail.TextColor = Colors.Black;
        }
    }

    private bool ValidateEntries()
    {
        bool validEntries = true;

        if (string.IsNullOrWhiteSpace(EntryEmail.Text))
        {
            EntryEmail.Placeholder = "Zadajte e-mail";
            EntryEmail.PlaceholderColor = Colors.OrangeRed;

            validEntries = false;
        }

        if (string.IsNullOrWhiteSpace(EntryPassword.Text))
        {
            EntryPassword.IsPassword = false;
            EntryPassword.Placeholder = "Zadajte heslo";
            EntryPassword.PlaceholderColor = Colors.OrangeRed;

            validEntries = false;
        }

        return validEntries;
    }

    private async void BtnSignInAsUser_Clicked(object sender, EventArgs e)
    {
        App.AppMode = AppMode.Customer;

        if (ValidateEntries())
        {
            await LoginGeneric(EntryEmail.Text, EntryPassword.Text);
        }
    }

    private async void BtnSignInAsCompany_Clicked(object sender, EventArgs e)
    {
        App.AppMode = AppMode.Company;

        if (ValidateEntries())
        {
            await LoginGeneric(EntryEmail.Text, EntryPassword.Text);
        }
    }

    private async Task LoginGeneric(string email, string password)
    {
        // this.Content = this._popUpIndic;

        (UserLoginGenericResponse UserLoginDTO, ExceptionHandler exception) response = await _loginService.LoginGeneric(EntryEmail.Text, EntryPassword.Text);

        if (response.UserLoginDTO != null)
        {
            App.UserData.UserSessionInfo.JWT = response.UserLoginDTO.JWT;
            App.UserData.UserSessionInfo.Email = response.UserLoginDTO.Email;

            App.UserData.UserLoginInfo.Email = EntryEmail.Text;
            App.UserData.UserLoginInfo.Password = EntryPassword.Text;

            if (await SettingsService.ContainsStaticAsync(nameof(App.UserData.UserLoginInfo)))
            {
                await SettingsService.RemoveStaticAsync(nameof(App.UserData.UserLoginInfo));
            }

            if (await SettingsService.ContainsStaticAsync(PrefUserSettings.PrefRememberLogIn))
            {
                if (await SettingsService.GetStaticAsync<bool>(PrefUserSettings.PrefRememberLogIn, false))
                {
                    string userLoginInfoSerialized = JsonConvert.SerializeObject(App.UserData.UserLoginInfo);
                    await SettingsService.SaveStaticAsync<string>(nameof(App.UserData.UserLoginInfo), userLoginInfoSerialized);
                }
            }

            // navigate to main view of certian AppMode
            //this.Content = this.MainControlWrapper;
            //await Shell.Current.GoToAsync($"//{nameof(CategoryPickerView)}");
        }
        else
        {
            //this.Content = this.MainControlWrapper;
            await DisplayAlert(App.LanguageResourceManager["LogInView_LogInError"].ToString(), response.exception.GetMessage, App.LanguageResourceManager["LogInView_Close"].ToString());
        }
    }

    private async Task LogInWithAuthProvider(string provider)
    {

        (UserLoginAuthProviderResponse UserLoginDTO, ExceptionHandler exception) response = await _loginService.LoginWithAuthProvider(provider);

        if (response.UserLoginDTO != null)
        {
            _webView = new WebView
            {
                UserAgent = "Mozilla/5.0 (Linux; Android 4.1.1; Galaxy Nexus Build/JRO03C) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Mobile Safari/535.19",
                Source = new UrlWebViewSource 
                { 
                    Url = "https://accounts.google.com/o/oauth2/auth?response_type=code&client_id=478494221197-lgnhi5pme4ia4mcj3cah5a2h0vip9st5.apps.googleusercontent.com&redirect_uri=https%3A%2F%2Flocalhost%3A7162%2Fapi%2FLogIn%2FRedirectUri&scope=openid%20email%20profile&state=abc123&access_type=offline&prompt=consent\r\n"
                    //response.UserLoginDTO.WebPage 
                }
            };

            await Navigation.PushAsync(new ContentPage { Content = _webView });
        }
        else
        {
            await DisplayAlert(App.LanguageResourceManager["LogInView_LogInError"].ToString(), response.exception.GetMessage, App.LanguageResourceManager["LogInView_Close"].ToString());
        }
    }

    private async void BtnGoogleSignInAsUser_Clicked(object sender, EventArgs e)
    {
        await LogInWithAuthProvider(AuthProviders.Google);
    }
}