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
using System;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using ZivnostAPI.Data.CusDbContext;
using ZivnostAPI.Models.AuthProvidersData;
using ZivnostAPI.Models.AuthProvidersData.Facebook;
using ZivnostAPI.Models.AuthProvidersData.Google;
using ZivnostAPI.Models.DatabaseModels.Account;

namespace ZivnostAPI.Services.LogInService;

public class LogInService : ILogInService
{
    private readonly CusDbContext _dataContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceProvider _serviceProvider;

    public LogInService(IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider, CusDbContext dataContext)
    {
        _httpClientFactory = httpClientFactory;
        _dataContext = dataContext;
        _serviceProvider = serviceProvider;
    }

    public async Task<ApiResponse<OAuthLandingPageResponse?>> GetAuthProviderLandingPage(AuthProviderLandingPageRequest request)
    {
        ApiResponse<OAuthLandingPageResponse?> response = new ApiResponse<OAuthLandingPageResponse?>();
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
                    response.Data = new OAuthLandingPageResponse
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

    public async Task<ApiResponse<UserOAuthResponse>> RedirectUri(IQueryCollection query)
    {
        ApiResponse<UserOAuthResponse> response = new ApiResponse<UserOAuthResponse>();
        response.Data = new UserOAuthResponse();

        string code = query["code"].ToString();
        string provider = query["state"].ToString();

        if (!string.IsNullOrEmpty(code))
        {
            HttpClient httpClient;
            Dictionary<string, string> tokenRequestData;
            string endPoint = string.Empty;

            OAuth oauthSettings = _serviceProvider.GetRequiredService<IOptions<OAuth>>().Value;

            if (provider == AuthProviders.Google)
            {
                httpClient = _httpClientFactory.CreateClient(AuthProviders.Google);

                tokenRequestData = new Dictionary<string, string>
                {
                    { "code", code },
                    { "client_id", $"{oauthSettings.Google.ClientId}" },
                    { "client_secret", $"{oauthSettings.Google.ClientSecret}" },
                    { "redirect_uri", $"{oauthSettings.RedirectUri}"},
                    { "grant_type", "authorization_code" }
                };

                endPoint = $"{oauthSettings.Google.BaseUrl}/token";
            }
            else if (provider == AuthProviders.Facebook)
            {
                httpClient = _httpClientFactory.CreateClient(AuthProviders.Facebook);

                tokenRequestData = new Dictionary<string, string>
                {
                    { "client_id", oauthSettings.Facebook.ClientId },
                    { "redirect_uri", oauthSettings.RedirectUri },
                    { "client_secret", oauthSettings.Facebook.ClientSecret },
                    { "code", code }
                };

                endPoint = "https://graph.facebook.com/v12.0/oauth/access_token";
            }
            else
            {
                throw new Exception($"Provider {provider} not supported");
            }

            FormUrlEncodedContent requestContent = new FormUrlEncodedContent(tokenRequestData);
            HttpResponseMessage tokenResponse = await httpClient.PostAsync(endPoint, requestContent);

            if (tokenResponse.IsSuccessStatusCode)
            {
                object? tokenData = null;
                if (provider == AuthProviders.Google)
                {
                    string responseContent = await tokenResponse.Content.ReadAsStringAsync();
                    tokenData = JsonConvert.DeserializeObject<GoogleTokenResponse?>(responseContent);
                }

                if (provider == AuthProviders.Facebook)
                {
                    string responseContent = await tokenResponse.Content.ReadAsStringAsync();
                    tokenData = JsonConvert.DeserializeObject<FacebookTokenResponse?>(responseContent);
                }

                ApiResponse<OAuthUserInfo> userInfo = await GetUserInfoFromOAuthProvider(httpClient, tokenData, provider);

                if (userInfo.Success && userInfo.Data != null)
                {
                    Account? account = await OAuthGetUserAccount(userInfo.Data, provider);

                    if (account == null)
                    {
                        if (await OAuthCreateUserAccount(userInfo.Data, account))
                        {
                            //new_user = true
                            response.Success = true;
                        }
                        else
                        {
                            response.Success = false;
                            response.ExceptionMessage = "Could not create user"; // todo get from above function and error message (tuple)
                        }
                    }
                    else
                    {
                        response.Data.SureName = account.SureName;

                        UserOAuthResponse(response.Data, account, tokenData);
                    }
                }
                else
                {
                    // failed to get user info data
                }
            }
            else
            {
                response.Success = false;
                response.ExceptionMessage = $"&exception=Failed to retrieve token data from provider: {provider}";
            }
        }
        else
        {
            response.Success = false;
            response.ExceptionMessage = $"&exception=Authorization code from {provider} not received";
        }

        return response;
    }

    public async Task<ApiResponse<OAuthUserInfo>> GetUserInfoFromOAuthProvider<T>(HttpClient httpClient, T oAuthTokenResponse, string provider)
    {
        ApiResponse<OAuthUserInfo> result = new ApiResponse<OAuthUserInfo>();
        result.Data = new OAuthUserInfo();

        try
        {
            string? accessToken = provider switch
            {
                AuthProviders.Google when oAuthTokenResponse is GoogleTokenResponse googleToken => googleToken.AccessToken,
                AuthProviders.Facebook when oAuthTokenResponse is FacebookTokenResponse facebookToken => facebookToken.AccessToken,
                _ => throw new ArgumentException("Invalid provider or token type")
            };

            string userInfoUrl = provider switch
            {
                AuthProviders.Google => "https://www.googleapis.com/oauth2/v2/userinfo",
                AuthProviders.Facebook => $"https://graph.facebook.com/me?fields=id,name,email,picture,phone&access_token={accessToken}",
                _ => throw new ArgumentException("Invalid provider")
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage userInfoResponse = await httpClient.GetAsync(userInfoUrl);

            if (userInfoResponse.IsSuccessStatusCode)
            {
                string userInfoResString = await userInfoResponse.Content.ReadAsStringAsync();

                if (!userInfoResString.IsNullOrEmpty())
                {
                    if (provider == AuthProviders.Google)
                    {
                        GoogleUserInfo? userInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(userInfoResString);
                        result.Data.GoogleUserInfo = userInfo;
                        result.Success = true;
                    }
                    else if (provider == AuthProviders.Facebook)
                    {
                        FacebookUserInfo? userInfo = JsonConvert.DeserializeObject<FacebookUserInfo>(userInfoResString);
                        result.Data.FacebookUserInfo = userInfo;
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.ExceptionMessage = $"Provider {provider} not supported";
                    }
                }
                else
                {
                    result.Success = false;
                    result.ExceptionMessage = $"User info data were not correctly retrieved from provider: {provider}";
                }
            }
            else
            {
                result.Success = false;
                result.ExceptionMessage = $"User info data were not correctly retrieved from provider: {provider}";
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ExceptionMessage = $"User info data were not correctly retrieved from provider: {provider}.\r\nError: {ex.Message}";
        }

        return result;
    }

    public async Task<Account?> OAuthGetUserAccount(OAuthUserInfo userInfo, string provider)
    {
        Func<Account, bool> predicate = provider switch
        {
            AuthProviders.Google => account => account.CommonId == userInfo.GoogleUserInfo?.Id,
            AuthProviders.Facebook => account => account.CommonId == userInfo.FacebookUserInfo?.Id,
            _ => account => false // Fallback in case the provider does not match
        };

        return await _dataContext.Account.AsQueryable().FirstOrDefaultAsync(account => predicate(account));
    }

    public async Task<bool> OAuthCreateUserAccount(OAuthUserInfo userInfo, Account? account)
    {
        if (userInfo.GoogleUserInfo != null)
        {
            account = new Account
            {
                IsCompanyAccount = false, // Pre-set Company to false (updated after user choose app mode during registration)

                CommonId = userInfo.GoogleUserInfo.Id,
                Email = userInfo.GoogleUserInfo.Email,
                Name = userInfo.GoogleUserInfo.Name,
                SureName = userInfo.GoogleUserInfo.FamilyName,
                PictureURL = userInfo.GoogleUserInfo.Picture,
            };
        }
        
        if (userInfo.FacebookUserInfo != null)
        {
            account = new Account
            {
                IsCompanyAccount = false, // Pre-set Company to false (updated after user choose app mode during registration)

                CommonId = userInfo.FacebookUserInfo.Id,
                Email = userInfo.FacebookUserInfo.Email,
                Name = userInfo.FacebookUserInfo.Name,
                MiddleName = userInfo.FacebookUserInfo.MiddleName,
                SureName = userInfo.FacebookUserInfo.LastName,
                PictureURL = userInfo.FacebookUserInfo.Picture.PictureData.Url
            };
        }

        if (account != null)
        {
            _dataContext.Account.Add(account);
        }

        return await _dataContext.SaveChangesAsync() > 0;
    }

    public void UserOAuthResponse(UserOAuthResponse oAuthResponse, Account account, object tokenData)
    {
        oAuthResponse.CommonId = account.CommonId;
        oAuthResponse.Email = account.Email;
        oAuthResponse.Phone = account.Phone;
        oAuthResponse.PictureURL = account.PictureURL;
        oAuthResponse.SureName = account.SureName;
        oAuthResponse.MiddleName = account.MiddleName;
        oAuthResponse.SureName = account.SureName;

        if (tokenData is GoogleTokenResponse googleToken)
        {
            oAuthResponse.OAuthAccessToken = googleToken.AccessToken;
            oAuthResponse.OauthRefreshToken = googleToken.RefreshToken;
            oAuthResponse.OAuthExpiresIn = googleToken.ExpiresIn;
        }

        if (tokenData is FacebookTokenResponse facebookToken)
        {
            oAuthResponse.OAuthAccessToken = facebookToken.AccessToken;
            oAuthResponse.OAuthExpiresIn = facebookToken.ExpiresIn;
        }
    }
}
