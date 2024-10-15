using A.Interfaces;
using A.Models.OAuthLoginData;
using A.Models.OAuthTokenData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SharedTypesLibrary.Models.AuthProvidersData.Google;
using SharedTypesLibrary.Models.AuthProvidersData.Facebook;
using SharedTypesLibrary.Models.RefreshTokenRequest;
using SharedTypesLibrary.ServiceResponseModel;
using ExceptionsHandling;
using System.Net.Http.Json;
using System.Text.Json;
using SharedTypesLibrary.DTOs.Response;
using ExtensionsLibrary.Http;

namespace A.Services;

class OauthService : IOauthService
{
    IHttpClientFactory _httpClientFactory;
    public OauthService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task ReloadUserDataFromOAuthProvider(ITokenData tokenData)
    {
        string userInfoUrl = tokenData switch
        {
            GoogleTokenData => "https://www.googleapis.com/oauth2/v2/userinfo",
            FacebookTokenData => $"https://graph.facebook.com/me?fields=id,name,email,picture,phone",
            _ => throw new ExceptionHandler("UAE_713", App.UserData.CurrentCulture)
        };

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);
            HttpResponseMessage userInfoResponse = await httpClient.GetAsync(userInfoUrl);

            if (userInfoResponse.IsSuccessStatusCode)
            {
                string userInfoResString = await userInfoResponse.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(userInfoResString))
                {
                    if (tokenData is GoogleTokenData)
                    {
                        GoogleUserInfo? userInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(userInfoResString);

                        if (userInfo != null)
                        {
                            App.UserData.UserIdentityData.Name = userInfo.GivenName;
                            App.UserData.UserIdentityData.SureName = userInfo.FamilyName;
                            App.UserData.UserIdentityData.Email = userInfo.Email;
                            App.UserData.UserIdentityData.PictureURL = userInfo.Picture;
                        }
                    }

                    if (tokenData is FacebookTokenData)
                    {
                        FacebookUserInfo? userInfo = JsonConvert.DeserializeObject<FacebookUserInfo>(userInfoResString);

                        if (userInfo != null)
                        {
                            App.UserData.UserIdentityData.Name = userInfo.Name;
                            App.UserData.UserIdentityData.MiddleName = userInfo.MiddleName;
                            App.UserData.UserIdentityData.SureName = userInfo.LastName;
                            App.UserData.UserIdentityData.Email = userInfo.Email;
                            App.UserData.UserIdentityData.Phone = userInfo.Phone;
                            App.UserData.UserIdentityData.PictureURL = userInfo.Picture.PictureData.Url;
                        }
                    }
                }
            }
        }
    }

    public async Task<(RefreshTokenResponse? refreshToken, ExceptionHandler? Exception)> RefreshAccessToken(RefreshTokenRequest request)
    {
        (RefreshTokenResponse? refreshToken, ExceptionHandler? Exception) result;

        HttpClient httpClient = _httpClientFactory.CreateClient();
        HttpResponseMessage response = await httpClient.PostAsJsonAsync<RefreshTokenRequest>("/api/OAuthController/RefreshAccessToken", request, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        if (response.IsSuccessStatusCode)
        {
            ApiResponse<RefreshTokenResponse> serializedResponse = await response.Content.ExtReadFromJsonAsync<RefreshTokenResponse>();
            result = (serializedResponse.Data, null);
        }
        else if ((!response.IsSuccessStatusCode) && (response.StatusCode == System.Net.HttpStatusCode.BadRequest))
        {
            ApiResponse<RefreshTokenResponse> serializedResponse = await response.Content.ExtReadFromJsonAsync<RefreshTokenResponse>();
            result = (null, new ExceptionHandler("UAE_004", serializedResponse.ApiErrorCode, serializedResponse.APIException, App.UserData.CurrentCulture));
        }
        else if (!response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ExtReadAsStringAsync();
            result = ExceptionHandler.ReadGenericHttpErrors<RefreshTokenResponse>(type: null, responseString: responseString, culture: App.UserData.CurrentCulture);
        }
        else
        {
            result = (null, new ExceptionHandler("UAE_400", App.UserData.CurrentCulture));
        }

        return result;
    }

}
