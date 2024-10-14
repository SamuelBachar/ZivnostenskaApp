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

using ExtensionsLibrary.Http;
using System.Runtime.InteropServices;
using System.Web;
using static Microsoft.Maui.ApplicationModel.Permissions;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace A.Services;

public class LoginService : ILoginService
{
    private readonly HttpClient _httpClient;

    public LoginService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(Constants.AppConstants.HttpsClientName);
    }

    public async Task<(UserLoginGenericResponse? UserInfo, ExceptionHandler? Exception)> LoginGeneric(string email, string passWord)
    {
        (UserLoginGenericResponse? UserInfo, ExceptionHandler? Exception) result = (null, null);

        try
        {
            UserLoginRequest userLoginRequest = new UserLoginRequest { Email = email, Password = passWord };
            var response = await _httpClient.PostAsJsonAsync($"/api/User/login", userLoginRequest, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (response.IsSuccessStatusCode)
            {
                ApiResponse<UserLoginGenericResponse> serializedResponse = await response.Content.ExtReadFromJsonAsync<UserLoginGenericResponse>();
                result = (serializedResponse.Data, null);
            }
            else if ((!response.IsSuccessStatusCode) && (response.StatusCode == System.Net.HttpStatusCode.BadRequest))
            {
                ApiResponse<UserLoginGenericResponse> serializedResponse = await response.Content.ExtReadFromJsonAsync<UserLoginGenericResponse>();
                result = (null, new ExceptionHandler("UAE_004", serializedResponse.ApiErrorCode, App.UserData.CurrentCulture));
            }
            else if (!response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ExtReadAsStringAsync();
                result = ExceptionHandler.ReadGenericHttpErrors<UserLoginGenericResponse>(type: null, responseString: responseString, culture: App.UserData.CurrentCulture);
            }
            else
            {
                result = (null, new ExceptionHandler("UAE_400", App.UserData.CurrentCulture));
            }
        }
        catch (ExceptionHandler ex)
        {
            result = (null, new ExceptionHandler(ex.ErrorCodeGeneric, null, extraErrors: ex.Message, App.UserData.CurrentCulture));
        }
        catch (Exception ex)
        {
            result = (null, new ExceptionHandler("UAE_401", null, extraErrors: ex.Message, App.UserData.CurrentCulture));
        }

        return result;
    }

    public async Task<(UserOAuthResponse? UserInfo, ExceptionHandler? Exception)> LoginWithAuthProvider(string provider)
    {
        (UserOAuthResponse? UserInfo, ExceptionHandler? Exception) result = (null, null);

        try
        {
            AuthProviderLandingPageRequest userAuthLoginRequest = new AuthProviderLandingPageRequest { Provider = provider };
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"/api/LogIn/GetAuthProviderLandingPage", userAuthLoginRequest, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (response.IsSuccessStatusCode)
            {
                ApiResponse<OAuthLandingPageResponse>? urlResult = await response.Content.ExtReadFromJsonAsync<OAuthLandingPageResponse>();

                WebAuthenticatorResult? resultWA = null;

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

                var oAuthResponse = CheckOAuthResponse(resultWA.Properties);

                if (oAuthResponse.isSuccess)
                {
                    UserOAuthResponse data = ParseUserOAuthResponse(resultWA.Properties);
                    result = (data, null);
                }
                else
                {
                    result = (null, new ExceptionHandler("UAE_005", oAuthResponse.apiErrorCode, oAuthResponse.exceptionMsg,
                            new Dictionary<string, string> { { "#provider", provider } }, App.UserData.CurrentCulture));
                }

            }
            else if ((!response.IsSuccessStatusCode) && (response.StatusCode == System.Net.HttpStatusCode.BadRequest))
            {
                var serializedResponse = await response.Content.ExtReadFromJsonAsync<ApiResponse<UserOAuthResponse>>();
                result = (null, new ExceptionHandler("UAE_004", serializedResponse.ApiErrorCode, App.UserData.CurrentCulture));
            }
            else if (!response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ExtReadAsStringAsync();
                result = ExceptionHandler.ReadGenericHttpErrors<UserOAuthResponse>(type: null, responseString: responseString, culture: App.UserData.CurrentCulture);
            }
            else
            {
                result = (null, new ExceptionHandler("UAE_400", App.UserData.CurrentCulture));
            }

        }
        catch (Exception ex)
        {
            result = (null, new ExceptionHandler("UAE_401", null, extraErrors: ex.Message, App.UserData.CurrentCulture));
        }

        return result;
    }

    public (bool isSuccess, string apiErrorCode, string exceptionMsg) CheckOAuthResponse(Dictionary<string, string> properties)
    {
        (bool, string, string) result;

        string success = properties.TryGetValue("success", out string? strSuccess) ? strSuccess : string.Empty;

        if (success == "true")
        {
            result = (true, string.Empty, string.Empty);
        }
        else
        {
            string exceptionMsg = properties.TryGetValue("exception", out string? strExc) ? strExc : string.Empty;
            string apiErrorCode = properties.TryGetValue("apiErrorCode", out string? strApiErrorCode) ? strApiErrorCode : string.Empty;

            result = (false, apiErrorCode, exceptionMsg);
        }

        return result;
    }

    public UserOAuthResponse ParseUserOAuthResponse(Dictionary<string, string> properties)
    {
        return new UserOAuthResponse
        {
            Id = properties.TryGetValue(OAuthUrlParamsResponse.Id, out string? id) ?  int.Parse(HttpUtility.UrlDecode(id)) : 0,
            OAuthId = properties.TryGetValue(OAuthUrlParamsResponse.OAuthId, out string? oAuthId) ? HttpUtility.UrlDecode(oAuthId) : string.Empty,
            Email = properties.TryGetValue(OAuthUrlParamsResponse.Email, out string? email) ? HttpUtility.UrlDecode(email) : string.Empty,
            Phone = properties.TryGetValue(OAuthUrlParamsResponse.Phone, out string? phone) ? HttpUtility.UrlDecode(phone) : string.Empty,
            PictureURL = properties.TryGetValue(OAuthUrlParamsResponse.PictureUrl, out string? pictureUrl) ? HttpUtility.UrlDecode(pictureUrl) : string.Empty,
            Name = properties.TryGetValue(OAuthUrlParamsResponse.Name, out string? name) ? HttpUtility.UrlDecode(name) : string.Empty,
            MiddleName = properties.TryGetValue(OAuthUrlParamsResponse.MiddleName, out string? middleName) ? HttpUtility.UrlDecode(middleName) : string.Empty,
            SureName = properties.TryGetValue(OAuthUrlParamsResponse.SureName, out string? sureName) ? HttpUtility.UrlDecode(sureName) : string.Empty,
            OAuthAccessToken = properties.TryGetValue(OAuthUrlParamsResponse.OAuthAccessToken, out string? accessToken) ? HttpUtility.UrlDecode(accessToken) : string.Empty,
            OauthRefreshToken = properties.TryGetValue(OAuthUrlParamsResponse.OAuthRefreshToken, out string? refreshToken) ? HttpUtility.UrlDecode(refreshToken) : string.Empty,
            OAuthExpiresIn = properties.TryGetValue(OAuthUrlParamsResponse.OAuthExpiresIn, out string? expiresIn) ? int.Parse(HttpUtility.UrlDecode(expiresIn)) : 0,
            NewUser = properties.TryGetValue(OAuthUrlParamsResponse.NewUser, out string? newUser) && newUser.ToLower() == "true"
        };
    }
}