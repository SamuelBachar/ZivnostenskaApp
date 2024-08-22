using A.Constants;
using A.Interfaces;
using A.Models;
using ExceptionsHandling;
using Newtonsoft.Json;
using SharedTypesLibrary.Constants;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

    public async Task<(UserLoginGenericResponse? UserInfo, ExceptionHandler? Exception)> LoginGeneric(string email, string passWord)
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
                    var serializedResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserLoginGenericResponse>>();

                    if (serializedResponse?.Data == null)
                    {
                        string serErrorMsg = new ExceptionHandler("UAE_902", App.UserData.CurrentCulture).CustomMessage;
                        throw new Exception(serErrorMsg);
                    }
                    else
                    {
                        return (new UserLoginGenericResponse { Email = serializedResponse.Data.Email, JWT = serializedResponse.Data.JWT }, null);
                    }
                }
                else if ((response.StatusCode == System.Net.HttpStatusCode.BadRequest) && (!response.IsSuccessStatusCode))
                {
                    var serializedResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserLoginGenericResponse>>();

                    if (serializedResponse?.Data == null)
                    {
                        string serErrorMsg = new ExceptionHandler("UAE_902", App.UserData.CurrentCulture).CustomMessage;
                        throw new Exception(serErrorMsg);
                    }
                    else
                    {
                        return (null, new ExceptionHandler("UAE_004", extraErrors: serializedResponse.Message, App.UserData.CurrentCulture));
                    }
                }
                else if (!response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (responseString.Contains("errors"))
                    {
                        return ExceptionHandler.ReadGenericHttpErrors<UserLoginGenericResponse>(type: null, responseString: responseString, culture: App.UserData.CurrentCulture);
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

    public async Task<(UserLoginAuthProviderResponse UserInfo, ExceptionHandler? Exception)> LoginWithAuthProvider(string provider)
    {
        try
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);

            UserLoginAuthProviderRequest userAuthLoginRequest = new UserLoginAuthProviderRequest { Provider = provider };
            HttpResponseMessage response = await httpClient.PostAsJsonAsync($"/api/LogIn/GetAuthProviderLandingPage", userAuthLoginRequest, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (response.IsSuccessStatusCode)
            {
                ApiResponse<UserLoginAuthProviderResponse> urlResult = await response.Content.ReadFromJsonAsync<ApiResponse<UserLoginAuthProviderResponse>>();

                WebAuthenticatorResult result = await WebAuthenticator.AuthenticateAsync
                (
                    new Uri(urlResult.Data.OAuthUrl.Replace(" ", "%20")),
                    new Uri($"{AuthProviderCallBackDataSchemes.MobileCallBackDataScheme}")
                );

                string accessToken = result?.AccessToken;

                if (result.Properties["success"] == "true")
                {
                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
                else if ((response.StatusCode == System.Net.HttpStatusCode.BadRequest) && (!response.IsSuccessStatusCode))
                {
                    var serializedResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserLoginGenericResponse>>();
                    return (null, new ExceptionHandler("UAE_004", extraErrors: serializedResponse.Message, App.UserData.CurrentCulture));
                }
                else if (!response.IsSuccessStatusCode)
                {

                }
            }
            else
            {
                return (null, new ExceptionHandler("UAE_005", extraErrors: response.Content.Properties["exception"], App.UserData.CurrentCulture));
            }
        }
        catch (Exception ex)
        {
            return (null, new ExceptionHandler("UAE_901", extraErrors: ex.Message, App.UserData.CurrentCulture));
        }

        return 
    }
}