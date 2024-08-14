using A.Constants;
using A.Interfaces;
using A.Models;
using A.Utils;
using ExceptionsHandling;
using Newtonsoft.Json;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.Request;
using SharedTypesLibrary.ServiceResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static A.Enums.Enums;

namespace A.Services;

public class LoginService : ILoginService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebAuthenticator _webAuthenticator;
    public LoginService(IHttpClientFactory httpClientFactory, IWebAuthenticator webAuthenticator)
    {
        _httpClientFactory = httpClientFactory;
        _webAuthenticator = webAuthenticator;
    }

    public async Task<(UserLoginDataDTO UserInfo, ExceptionHandler? Exception)> LoginGeneric(string email, string passWord)
    {
        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);

                UserLoginRequest userLoginRequest = new UserLoginRequest { Email = email, Password = passWord };
                var response = await httpClient.PostAsJsonAsync($"/api/User/login", userLoginRequest, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                if (response.IsSuccessStatusCode)
                {
                    var serializedResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<UserLoginDataDTO>>();
                    return (new UserLoginDataDTO { Email = serializedResponse.Data.Email, JWT = serializedResponse.Data.JWT }, null);
                }
                else if ((response.StatusCode == System.Net.HttpStatusCode.BadRequest) && (!response.IsSuccessStatusCode))
                {
                    var serializedResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<UserLoginDataDTO>>();
                    return (null, new ExceptionHandler("UAE_004", extraErrors: serializedResponse.Message, App.UserData.CurrentCulture));
                }
                else if (!response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (responseString.Contains("errors"))
                    {
                        Dictionary<string, List<string>> dicGenericErrors = GenericHttpErrorReader.ExtractErrorsFromWebAPIResponse(responseString);

                        var temp = string.Empty;

                        foreach (var error in dicGenericErrors)
                        {
                            foreach (var errorInfo in error.Value)
                                temp += errorInfo + "\r\n";
                        }
                        
                        return (null, new ExceptionHandler("UAE_901", extraErrors: temp, App.UserData.CurrentCulture));
                    }
                    else
                    {
                        return (null, new ExceptionHandler("UAE_900", App.UserData.CurrentCulture));
                    }
                }

                return (null, new ExceptionHandler("UAE_900", App.UserData.CurrentCulture));
            }
            else
            {
                return (null, new ExceptionHandler("UAE_003", App.UserData.CurrentCulture));
            }
        }
        catch (Exception ex)
        {
            return (null, new ExceptionHandler("UAE_901", extraErrors: ex.Message, App.UserData.CurrentCulture));
        }
    }

    public async Task<(UserLoginDataDTO UserInfo, ExceptionHandler? Exception)> LoginWithAuthProvider(AuthProvider provider)
    {
        try
        {
            if (provider == AuthProvider.Apple)
            {
                await AppleAuthenticate();
            }
            else if (provider == AuthProvider.Google || provider == AuthProvider.Facebook)
            {
                await GoogleFacebookAuthenticate();
            }
        }
        catch (Exception ex)
        {
            return (null, new ExceptionHandler("UAE_901", extraErrors: ex.Message, App.UserData.CurrentCulture));
        }

        return (null, new ExceptionHandler("UAE_901", "tmp", App.UserData.CurrentCulture));
    }

    private async Task AppleAuthenticate()
    {
        var scheme = "..."; // Apple, Microsoft, Google, Facebook, etc.
        var authUrlRoot = "https://mysite.com/mobileauth/";
        WebAuthenticatorResult result = null;

        if ( DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Version.Major >= 13)
        {
            // Use Native Apple Sign In API's
            result = await AppleSignInAuthenticator.AuthenticateAsync();
        }
        else
        {
            // Web Authentication flow
            var authUrl = new Uri($"{authUrlRoot}{scheme}");
            var callbackUrl = new Uri("myapp://");

            result = await WebAuthenticator.Default.AuthenticateAsync(authUrl, callbackUrl);
        }

        var authToken = string.Empty;

        if (result.Properties.TryGetValue("name", out string name) && !string.IsNullOrEmpty(name))
            authToken += $"Name: {name}{Environment.NewLine}";

        if (result.Properties.TryGetValue("email", out string email) && !string.IsNullOrEmpty(email))
            authToken += $"Email: {email}{Environment.NewLine}";

        // Note that Apple Sign In has an IdToken and not an AccessToken
        authToken += result?.AccessToken ?? result?.IdToken;
    }

    private async Task GoogleFacebookAuthenticate()
    {
        // TODO tu je len jedno konkretne client_id a to pre debug apk cize not real signed a pre Android a GoogleAuth..
        // treba dalsie aj pre Apple, Facebook atd.. takze treba tu vymysliet lepsi sposob rozoznat na akej som platforme a vytihanut na zaklade
        // toho z configu spravne OAuth Client Id

        var callbackUrl = "com.majster.majster_app://"; // "myapp://"

        try
        {
            Uri Url1 = new Uri(new Uri("https://localhost:7162/"), "mobileauth/signingoogle");
                WebAuthenticatorResult authResult = await _webAuthenticator.AuthenticateAsync(
                new WebAuthenticatorOptions()
                {
                    Url = Url1,
                    CallbackUrl = new Uri(callbackUrl),
                    PrefersEphemeralWebBrowserSession = true
                });

            if (authResult != null)
            {
                string accessToken = authResult.AccessToken;

                var parameters = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("grant_type","authorization_code"),
                    new KeyValuePair<string,string>("client_id","478494221197-sohbnoje2ve5ak0aik1jovaostffl4jb.apps.googleusercontent.com"),
                    new KeyValuePair<string,string>("redirect_uri",callbackUrl),
                    new KeyValuePair<string,string>("code", accessToken)
                });


                HttpClient client = new HttpClient();
                HttpResponseMessage? accessTokenResponse = await client.PostAsync("https://oauth2.googleapis.com/token", parameters);

                LoginResponse loginResponse;

                if (accessTokenResponse.IsSuccessStatusCode)
                {
                    var data = await accessTokenResponse.Content.ReadAsStringAsync();

                    loginResponse = JsonConvert.DeserializeObject<LoginResponse>(data);
                }
                else
                {

                }
            }

            // Do something with the token
        }
        catch (TaskCanceledException e)
        {
            int a = 2;
            int b =+ a;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
