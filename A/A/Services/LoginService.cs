using A.Constants;
using A.Interfaces;
using A.Models;
using A.Utils;
using ExceptionsHandling;
using Newtonsoft.Json;
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

    public async Task<(UserLoginGenericResponse UserInfo, ExceptionHandler? Exception)> LoginGeneric(string email, string passWord)
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
                    return (new UserLoginGenericResponse { Email = serializedResponse.Data.Email, JWT = serializedResponse.Data.JWT }, null);
                }
                else if ((response.StatusCode == System.Net.HttpStatusCode.BadRequest) && (!response.IsSuccessStatusCode))
                {
                    var serializedResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserLoginGenericResponse>>();
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

    public async Task<(UserLoginAuthProviderResponse UserInfo, ExceptionHandler? Exception)> LoginWithAuthProvider(string provider)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);

            UserLoginAuthProviderRequest userAuthLoginRequest = new UserLoginAuthProviderRequest { Provider = provider };
            var response = await httpClient.PostAsJsonAsync($"/api/LogIn/GetAuthProviderLandingPage", userAuthLoginRequest, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserLoginAuthProviderResponse>>();
                 
                // Display webView in your page
                return (result.Data, null);
            }
        }
        catch (Exception ex)
        {
            return (null, new ExceptionHandler("UAE_901", extraErrors: ex.Message, App.UserData.CurrentCulture));
        }

        return (null, new ExceptionHandler("UAE_901", "tmp", App.UserData.CurrentCulture));
    }
}