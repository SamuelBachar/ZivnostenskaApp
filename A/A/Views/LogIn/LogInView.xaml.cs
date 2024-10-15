using A.AppPreferences;
using A.Constants;
using A.Interfaces;
using A.Models.OAuthLoginData;
using A.Models.OAuthTokenData;
using A.Services;
using A.Views.LogIn;
using A.Views.Register;
using ExceptionsHandling;
using Newtonsoft.Json;
using SharedTypesLibrary.Constants;
using SharedTypesLibrary.DTOs.Response;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using static A.Enumerations.Enums;
using static System.Net.WebRequestMethods;

namespace A.Views;

public partial class LogInView : ContentPage
{
    public class UserLoginInfo
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    private readonly ILoginService _loginService;
    private readonly IOauthService _oAuthService;

    public LogInView(ILoginService loginService, IOauthService oAuthService)
    {
        InitializeComponent();

        _loginService = loginService;
        _oAuthService = oAuthService;
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
        await Shell.Current.GoToAsync($"{nameof(RegisterChooseView)}");
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
        (UserLoginGenericResponse UserLoginDTO, ExceptionHandler exception) response = await _loginService.LoginGeneric(EntryEmail.Text, EntryPassword.Text);

        if (response.UserLoginDTO != null)
        {
            App.UserData.UserAuthData.IsOAuthLogin = false;

            App.UserData.UserAuthData.JWT = response.UserLoginDTO.JWT;
            App.UserData.UserAuthData.JWTRefreshToken = response.UserLoginDTO.JWTRefreshToken;
            App.UserData.UserIdentityData.Email = response.UserLoginDTO.Email;

            UserLoginInfo userLoginInfo = new UserLoginInfo();
            userLoginInfo.Email = EntryEmail.Text;
            userLoginInfo.Password = EntryPassword.Text;

            if (await SettingsService.ContainsStaticAsync(nameof(UserLoginInfo)))
            {
                await SettingsService.RemoveStaticAsync(nameof(UserLoginInfo));
            }

            if (await SettingsService.ContainsStaticAsync(PrefUserSettings.PrefRememberLogIn))
            {
                if (await SettingsService.GetStaticAsync<bool>(PrefUserSettings.PrefRememberLogIn, false))
                {
                    string userLoginInfoSerialized = JsonConvert.SerializeObject(userLoginInfo);
                    await SettingsService.SaveStaticAsync<string>(nameof(UserLoginInfo), userLoginInfoSerialized);
                }
            }

            await Shell.Current.GoToAsync($"{nameof(MainPage)}");
        }
        else
        {
            //this.Content = this.MainControlWrapper;
            await DisplayAlert(App.LanguageResourceManager["LogInView_LogInError"].ToString(), response.exception.CustomMessage, App.LanguageResourceManager["AllView_Close"].ToString());
        }
    }

    private async void OnAuthProviderLogInRegister_Tapped(object sender, TappedEventArgs e)
    {
        (UserOAuthResponse? userLoginInfo, ExceptionHandler? exception) response;

        try
        {
            string authProvider = e.Parameter as string ?? "";
            ITokenData? tokenData = null;

            tokenData = await GetStoredAccessTokenData(authProvider);

            if (tokenData != null)
            {
                if (!IsAccessTokenExpired(tokenData))
                {
                    await _oAuthService.ReloadUserDataFromOAuthProvider(tokenData);
                    await NavigateToNextPage();
                }

                if (IsRefreshOfAccessTokenAllowed(authProvider))
                {

                    await NavigateToNextPage();
                }
                else
                {
                    response = await _loginService.LoginWithAuthProvider(authProvider);
                }
            }
            else
            {
                response = await _loginService.LoginWithAuthProvider(authProvider);

                if (response.userLoginInfo != null)
                {
                    await StoreOAuthResponseData(response.userLoginInfo, authProvider);
                    await NavigateToNextPage(response.userLoginInfo.NewUser);
                }
                else
                {
                    await DisplayAlert(App.LanguageResourceManager["LogInView_LogInError"].ToString(), response.exception?.CustomMessage ?? "", App.LanguageResourceManager["AllView_Close"].ToString());
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(App.LanguageResourceManager["LogInView_LogInError"].ToString(), ex.Message, App.LanguageResourceManager["AllView_Close"].ToString());
        }
    }

    private async Task StoreOAuthResponseData(UserOAuthResponse userLoginInfo, string authProvider)
    {
        App.UserData.UserAuthData.JWT = userLoginInfo.JWT;
        App.UserData.UserAuthData.JWTRefreshToken = userLoginInfo.JWTRefreshToken;

        App.UserData.UserAuthData.IsOAuthLogin = true;
        App.UserData.UserAuthData.Provider = authProvider;
        App.UserData.UserAuthData.OAuthAccessToken = userLoginInfo.OAuthAccessToken;
        App.UserData.UserAuthData.OAuthRefreshToken = userLoginInfo.OauthRefreshToken;
        App.UserData.UserAuthData.OAuthExpiresIn = userLoginInfo.OAuthExpiresIn;

        App.UserData.UserIdentityData.Id = userLoginInfo.Id;
        App.UserData.UserIdentityData.OAuthId = userLoginInfo.OAuthId;
        App.UserData.UserIdentityData.Email = userLoginInfo.Email;
        App.UserData.UserIdentityData.Phone = userLoginInfo.Phone;
        App.UserData.UserIdentityData.PictureURL = userLoginInfo.PictureURL;
        App.UserData.UserIdentityData.Name = userLoginInfo.Name;
        App.UserData.UserIdentityData.MiddleName = userLoginInfo.MiddleName;
        App.UserData.UserIdentityData.SureName = userLoginInfo.SureName;

        if (authProvider == AuthProviders.Google)
        {
            GoogleTokenData data = new GoogleTokenData
            {
                AccessToken = App.UserData.UserAuthData.OAuthAccessToken,
                RefreshToken = App.UserData.UserAuthData.OAuthRefreshToken,
                ValidUntil = new DateTime(App.UserData.UserAuthData.OAuthExpiresIn)
            };

            string jsonData = JsonConvert.SerializeObject(data);
            await SecureStorage.Default.SetAsync(nameof(GoogleTokenData), jsonData);
        }

        if (authProvider == AuthProviders.Facebook)
        {
            FacebookTokenData data = new FacebookTokenData
            {
                AccessToken = App.UserData.UserAuthData.OAuthAccessToken,
                ValidUntil = new DateTime(App.UserData.UserAuthData.OAuthExpiresIn)
            };

            string jsonData = JsonConvert.SerializeObject(data);
            await SecureStorage.Default.SetAsync(nameof(FacebookTokenData), jsonData);
        }

        if (authProvider == AuthProviders.Apple)
        {
            AppleTokenData data = new AppleTokenData
            {
                AccessToken = App.UserData.UserAuthData.OAuthAccessToken,
                ValidUntil = new DateTime(App.UserData.UserAuthData.OAuthExpiresIn)
            };

            string jsonData = JsonConvert.SerializeObject(data);
            await SecureStorage.Default.SetAsync(nameof(FacebookTokenData), jsonData);
        }
    }

    private async Task<ITokenData?> GetStoredAccessTokenData(string authProvider)
    {
        string? jsonData = string.Empty;
        ITokenData? tokenData = null;

        if (authProvider == AuthProviders.Google)
        {
            jsonData = await SecureStorage.Default.GetAsync(nameof(GoogleTokenData));

            if (!string.IsNullOrEmpty(jsonData))
            {
                tokenData = JsonConvert.DeserializeObject<GoogleTokenData>(jsonData);
            }
        }

        if (authProvider == AuthProviders.Facebook)
        {
            jsonData = await SecureStorage.Default.GetAsync(nameof(FacebookTokenData));

            if (!string.IsNullOrEmpty(jsonData))
            {
                tokenData = JsonConvert.DeserializeObject<FacebookTokenData>(jsonData);
            }
        }

        return tokenData;
    }

    private bool IsAccessTokenExpired(ITokenData tokenData)
    {
        bool result = false;

        if (tokenData is FacebookTokenData facebookData)
        {
            if (facebookData.ValidUntil > DateTime.Now.AddMinutes(-1))
            {
                result = true;
            }
        }

        if (tokenData is GoogleTokenData googleToken)
        {
            if (googleToken.ValidUntil > DateTime.Now.AddMinutes(-1))
            {
                result = true;
            }
        }

        return result;
    }

    private async Task NavigateToNextPage(bool newUser = false)
    {
        // Skiping choosing of Application Mode since user already choosed prefered Application Mode
        if (await SettingsService.GetStaticAsync<bool>(PrefUserSettings.PrefRememberAppModeChoice, false))
        {
            AppMode appMode = await SettingsService.GetStaticAsync<AppMode>(PrefUserSettings.AppModeChoice, AppMode.Customer);

            if (appMode == AppMode.Customer)
            {
                await Shell.Current.GoToAsync($"{nameof(MainPage)}");
            }
            else
            {
                await Shell.Current.GoToAsync($"{nameof(MainPage)}");
            }
        }
        else
        {
            // Navigate to LogInChooseView where application mode is choosen
            await Shell.Current.GoToAsync($"{nameof(LogInChooseView)}?NewUser={newUser}");
        }
    }

    private bool IsRefreshOfAccessTokenAllowed(string provider)
    {
        return provider == AuthProviders.Google || provider == AuthProviders.Apple;
    }

    private async Task RefreshAccessToken(string provider)
    {

    }
}