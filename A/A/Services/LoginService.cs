using A.Constants;
using A.Interfaces;
using A.Utils;
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

    public async Task<(UserLoginDataDTO UserInfo, string ResultMessage)> LoginHTTPS(string email, string passWord)
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
                    return (new UserLoginDataDTO { Email = serializedResponse.Data.Email, JWT = serializedResponse.Data.JWT }, serializedResponse.Message);
                }
                else if ((response.StatusCode == System.Net.HttpStatusCode.BadRequest) && (!response.IsSuccessStatusCode))
                {
                    var serializedResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<UserLoginDataDTO>>();
                    return (null, serializedResponse.Message);
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

                        return (null, temp);
                    }
                    else
                    {
                        return (null, ResultMessage: $"Neočakavaná odpoveď od servera, neznáma chyba");
                    }
                }

                return (null, ResultMessage: $"Neočakavaná odpoveď od servera, neznáma chyba");
            }
            else
            {
                return (null, ResultMessage: "Nie je možné sa prihlásiť\r\n. Zariadenie nemá pripojenie k internetu");
            }
        }
        catch (Exception ex)
        {
            return (null, $"Nastala chyba pri komunikácií so serverom. Chyba: {ex.Message}");
        }
    }
}
