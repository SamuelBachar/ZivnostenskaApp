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
    public LoginService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(UserLoginGenericResponse? UserInfo, ExceptionHandler? Exception)> LoginGeneric(string email, string passWord)
    {
        (UserLoginGenericResponse? UserInfo, ExceptionHandler? Exception) result = (null, null);

        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);

                UserLoginRequest userLoginRequest = new UserLoginRequest { Email = email, Password = passWord };
                var response = await httpClient.PostAsJsonAsync($"/api/User/login", userLoginRequest, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                if (response == null || response.Content == null)
                {
                    string serErrorMsg = new ExceptionHandler("UAE_903", App.UserData.CurrentCulture).CustomMessage;
                    throw new Exception(serErrorMsg);
                }

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
                        result = (new UserLoginGenericResponse { Email = serializedResponse.Data.Email, JWT = serializedResponse.Data.JWT }, null);
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
                        result = (null, new ExceptionHandler("UAE_004", extraErrors: serializedResponse.Message, App.UserData.CurrentCulture));
                    }
                }
                else if (!response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (responseString.Contains("errors"))
                    {
                        result = ExceptionHandler.ReadGenericHttpErrors<UserLoginGenericResponse>(type: null, responseString: responseString, culture: App.UserData.CurrentCulture);
                    }
                    else
                    {
                        result = (null, new ExceptionHandler("UAE_900", App.UserData.CurrentCulture));
                    }
                }

                result = (null, new ExceptionHandler("UAE_900", App.UserData.CurrentCulture));
            }
            else
            {
                result = (null, new ExceptionHandler("UAE_003", App.UserData.CurrentCulture));
            }
        }
        catch (Exception ex)
        {
            result = (null, new ExceptionHandler("UAE_901", extraErrors: ex.Message, App.UserData.CurrentCulture));
        }

        return result;
    }

    public async Task<(UserLoginAuthProviderResponse? UserInfo, ExceptionHandler? Exception)> LoginWithAuthProvider(string provider)
    {
        (UserLoginAuthProviderResponse? UserInfo, ExceptionHandler? Exception) result = (null, null);

        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                HttpClient httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);

                UserLoginAuthProviderRequest userAuthLoginRequest = new UserLoginAuthProviderRequest { Provider = provider };
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"/api/LogIn/GetAuthProviderLandingPage", userAuthLoginRequest, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                if (response == null || response.Content == null)
                {
                    string serErrorMsg = new ExceptionHandler("UAE_903", App.UserData.CurrentCulture).CustomMessage;
                    throw new Exception(serErrorMsg);
                }

                if (response.IsSuccessStatusCode)
                {
                    ApiResponse<UserLoginAuthProviderResponse>? urlResult = await response.Content.ReadFromJsonAsync<ApiResponse<UserLoginAuthProviderResponse>>();

                    if (urlResult == null)
                    {
                        string serErrorMsg = new ExceptionHandler("UAE_902", App.UserData.CurrentCulture).CustomMessage;
                        throw new Exception(serErrorMsg);
                    }
                    else
                    {
                        WebAuthenticatorResult resultWA = null;

                        if (provider == AuthProviders.Apple && DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Version.Major >= 13)
                        {
                            resultWA = await AppleSignInAuthenticator.AuthenticateAsync();
                        }
                        else
                        {
                            resultWA = await WebAuthenticator.AuthenticateAsync
                            (
                                new WebAuthenticatorOptions()
                                {
                                    Url = new Uri(urlResult.Data.OAuthUrl.Replace(" ", "%20")),
                                    CallbackUrl = new Uri($"{AuthProviderCallBackDataSchemes.MobileCallBackDataScheme}"),
                                    PrefersEphemeralWebBrowserSession = true // effective only on iOS
                                }
                            );
                        }

                        if (resultWA.Properties["success"] == "true")
                        {
                            bool newUser = (resultWA.Properties["new_user"] == "true");
                            string token = resultWA.AccessToken;

                            result = (new UserLoginAuthProviderResponse { Token = token, NewUser = newUser }, null);
                        }
                        else
                        {
                            result = (null, new ExceptionHandler("UAE_005", extraErrors: resultWA.Properties["exception"],
                                    new Dictionary<string, string> { { "#provider", provider } }, App.UserData.CurrentCulture));
                        }

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
                        result = (null, new ExceptionHandler("UAE_004", extraErrors: serializedResponse.Message, App.UserData.CurrentCulture));
                    }
                }
                else if (!response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (responseString.Contains("errors"))
                    {
                        result = ExceptionHandler.ReadGenericHttpErrors<UserLoginAuthProviderResponse>(type: null, responseString: responseString, culture: App.UserData.CurrentCulture);
                    }
                    else
                    {
                        result = (null, new ExceptionHandler("UAE_900", App.UserData.CurrentCulture));
                    }
                }
            }
            else
            {
                result = (null, new ExceptionHandler("UAE_003", App.UserData.CurrentCulture));
            }

        }
        catch (Exception ex)
        {
            result = (null, new ExceptionHandler("UAE_901", extraErrors: ex.Message, App.UserData.CurrentCulture));
        }

        return result;
    }
}