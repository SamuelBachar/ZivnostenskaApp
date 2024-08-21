using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using SharedTypesLibrary.Constants;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http.Headers;
using System.Text;
using ZivnostAPI.Data.DataContext;
using ZivnostAPI.Models.Account;
using ZivnostAPI.Models.AuthProvidersData;
using ZivnostAPI.Models.AuthProvidersData.Google;

namespace ZivnostAPI.Services.LogInService;

public class LogInService : ILogInService
{
    private readonly DataContext _dataContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceProvider _serviceProvider;

    public LogInService(IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider, DataContext dataContext)
    {
        _httpClientFactory = httpClientFactory;
        _dataContext = dataContext;
        _serviceProvider = serviceProvider;
    }

    public async Task<ApiResponse<UserLoginAuthProviderResponse?>> GetAuthProviderLandingPage(UserLoginAuthProviderRequest request)
    {
        ApiResponse<UserLoginAuthProviderResponse?> response = new ApiResponse<UserLoginAuthProviderResponse?>();
        HttpClient? httpClient = null;

        await Task.Run(() =>
        {
            if (request.Provider == AuthProviders.Google)
            {
                httpClient = _httpClientFactory.CreateClient(AuthProviders.Google);
            }
            else if (request.Provider == AuthProviders.Facebook)
            {
                httpClient = _httpClientFactory.CreateClient(AuthProviders.Facebook);
            }
            else if (request.Provider == AuthProviders.Apple)
            {
                httpClient = _httpClientFactory.CreateClient(AuthProviders.Apple);
            }

            if (httpClient != null)
            {
                if (httpClient.BaseAddress != null)
                {
                    response.Success = true;
                    response.Data = new UserLoginAuthProviderResponse
                    {
                        OAuthUrl = httpClient.BaseAddress.ToString()
                    };
                }
                else
                {
                    response.Success = false;
                    response.ExceptionMessage = "Unsupported provider";
                }
            }
            else
            {
                response.Success = false;
                response.ExceptionMessage = "Unsupported provider";
            }
        });

        return response;
    }

    public async Task<ApiResponse<UserLoginAuthProviderResponse?>> AuthenticateWithAuthProvider(UserLoginAuthProviderRequest request)
    {
        ApiResponse<UserLoginAuthProviderResponse?> response = new ApiResponse<UserLoginAuthProviderResponse?>();

        if (request.Provider == AuthProviders.Google)
        {
            var client = _httpClientFactory.CreateClient(AuthProviders.Google);
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");

            var tokenRequestData = new Dictionary<string, string>
            {
                { "code", request.Code },
                { "client_id", "YOUR_GOOGLE_CLIENT_ID" },
                { "client_secret", "YOUR_GOOGLE_CLIENT_SECRET" },
                { "redirect_uri", "YOUR_REDIRECT_URI" },
                { "grant_type", "authorization_code" }
            };

            tokenRequest.Content = new FormUrlEncodedContent(tokenRequestData);
            var tokenResponse = await client.SendAsync(tokenRequest);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenResponseData = await tokenResponse.Content.ReadAsStringAsync();
                // Process token, save user, etc.
                response.Success = true;
                response.Data = new UserLoginAuthProviderResponse { Token = tokenResponseData };
            }
            else
            {
                response.Success = false;
                response.Data = null;
            }
        }
        else
        {
            response.Success = false;
            response.Data = null;
        }

        return response;
    }

    public async Task<string> RedirectUri(IQueryCollection query)
    {
        string responseString = AuthProviderCallBackDataSchemes.MobileCallBackDataScheme;

        GoogleTokenResponse? tokenData = null;

        string code = query["code"].ToString();
        string provider = query["state"].ToString();

        if (!string.IsNullOrEmpty(code))
        {
            if (provider == AuthProviders.Google)
            {
                OAuth oauthGoogle = _serviceProvider.GetRequiredService<IOptions<OAuth>>().Value;

                var tokenRequestData = new Dictionary<string, string>
                {
                    { "code", code },
                    { "client_id", $"{oauthGoogle.Google.ClientId}" },
                    { "client_secret", $"{oauthGoogle.Google.ClientSecret}" },
                    { "redirect_uri", $"{oauthGoogle.RedirectUri}"},
                    { "grant_type", "authorization_code" }
                };

                FormUrlEncodedContent requestContent = new FormUrlEncodedContent(tokenRequestData);

                HttpClient client = _httpClientFactory.CreateClient(AuthProviders.Google);
                HttpResponseMessage tokenResponse = await client.PostAsync($"{oauthGoogle.Google.BaseUrl}/token", requestContent);

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    responseString += "?success=false";
                    responseString += "&exception=test123";
                }
                else
                {
                    string responseContent = await tokenResponse.Content.ReadAsStringAsync();
                    tokenData = JsonConvert.DeserializeObject<GoogleTokenResponse?>(responseContent);
                    responseString = await ProcessUserAuthentication(client, tokenData, responseString);
                }
            }
            else
            {
                responseString += "?success=false";
                responseString += "&exception=test1234";
            }

        }
        else
        {
            responseString += "?success=false";
            responseString += $"&exception=Provider not {provider} supported";
        }

        return responseString;
    }

    public async Task<string> ProcessUserAuthentication(HttpClient httpClient, GoogleTokenResponse? tokenData, string responseString)
    {
        try
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken); // TODO replace
            HttpResponseMessage userInfoResponse = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");

            if (userInfoResponse.IsSuccessStatusCode)
            {
                string userInfoResString = await userInfoResponse.Content.ReadAsStringAsync();

                if (!userInfoResString.IsNullOrEmpty())
                {
                    GoogleUserInfo userInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(userInfoResString);

                    // User not present in DB yet
                    Account? account = await _dataContext.Account.FirstOrDefaultAsync(account => account.CommonId == userInfo.Id);

                    if (account == null)
                    {
                        account = new Account
                        {
                            CommonId = userInfo.Id,
                            Email = userInfo.Email,
                            IsCompany = false, // Pre-set that account is not company
                        };

                        _dataContext.Account.Add(account);
                        await _dataContext.SaveChangesAsync();
                    }

                    responseString += $"?success=true";
                    responseString += $"&access_token={tokenData.AccessToken}";
                    responseString += $"&user_id={account.CommonId}";
                }
                else
                {
                    responseString += "?success=false";
                    responseString += "&exception=test12345";
                }
            }
            else
            {
                responseString += "?success=false";
                responseString += "&exception=test123456";
            }
        }
        catch (Exception ex)
        {
            responseString += "?success=false";
            responseString += $"&exception={ex.Message}";
        }

        // Redirect to your app with the relevant information
        return responseString;
    }
}
