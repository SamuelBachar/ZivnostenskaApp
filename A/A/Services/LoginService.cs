using A.Constants;
using A.Interfaces;
using A.Utils;
using ExceptionsHandling;
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

namespace A.Services;

public class LoginService : ILoginService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public LoginService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(UserLoginDataDTO UserInfo, ExceptionHandler? Exception)> LoginHTTPS(string email, string passWord)
    {
        //HTTPS
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
}
