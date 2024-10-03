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

    public async Task<ApiResponse<OAuthLandingPageResponse?>> GetAuthProviderLandingPage(UserLoginAuthProviderLandingPageRequest request)
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

    public async Task<string> RedirectUri(IQueryCollection query)
    {
        string responseString = AuthProviderCallBackDataSchemes.MobileCallBackDataScheme;

        string code = query["code"].ToString();
        string provider = query["state"].ToString();

        if (!string.IsNullOrEmpty(code))
        {
            if (provider == AuthProviders.Google)
            {
                HttpClient client = _httpClientFactory.CreateClient(AuthProviders.Google);
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

                HttpResponseMessage tokenResponse = await client.PostAsync($"{oauthGoogle.Google.BaseUrl}/token", requestContent);

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    responseString += "?success=false";
                    responseString += $"&exception=Failed to retrieve token data from provider: {provider}";
                }
                else
                {
                    GoogleTokenResponse? tokenData = null;
                    string responseContent = await tokenResponse.Content.ReadAsStringAsync();
                    tokenData = JsonConvert.DeserializeObject<GoogleTokenResponse?>(responseContent);
                    responseString = await GetUserInfoFromOAuthProvider(client, tokenGoogle: tokenData, tokenFacebook: null, AuthProviders.Google, responseString);
                }
            }
            else if (provider == AuthProviders.Facebook)
            {
                HttpClient client = _httpClientFactory.CreateClient(AuthProviders.Facebook);
                OAuth oauthFacebook = _serviceProvider.GetRequiredService<IOptions<OAuth>>().Value;

                var requestParams = new Dictionary<string, string>
                {
                    { "client_id", oauthFacebook.Facebook.ClientId },
                    { "redirect_uri", oauthFacebook.RedirectUri },
                    { "client_secret", oauthFacebook.Facebook.ClientSecret },
                    { "code", code }
                };

                FormUrlEncodedContent requestContent = new FormUrlEncodedContent(requestParams);
                HttpResponseMessage tokenResponse = await client.PostAsync($"https://graph.facebook.com/v12.0/oauth/access_token", requestContent);

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    responseString += "?success=false";
                    responseString += $"&exception=Failed to retrieve token data from provider: {provider}";
                }
                else
                {
                    FacebookTokenResponse? tokenData = null;
                    string responseContent = await tokenResponse.Content.ReadAsStringAsync();
                    tokenData = JsonConvert.DeserializeObject<FacebookTokenResponse?>(responseContent);
                    responseString = await GetUserInfoFromOAuthProvider(client, tokenGoogle: null, tokenFacebook: tokenData, AuthProviders.Facebook, responseString);
                }
            }
            else
            {
                responseString += "?success=false";
                responseString += $"&exception=Provider {provider} not supported";
            }

        }
        else
        {
            responseString += "?success=false";
            responseString += $"&exception=Authorization code from {provider} not received";
        }

        return responseString;
    }

    public async Task<string> GetUserInfoFromOAuthProvider(HttpClient httpClient, GoogleTokenResponse? tokenGoogle, FacebookTokenResponse? tokenFacebook, string provider, string responseString)
    {
        try
        {
            if (provider == AuthProviders.Google)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenGoogle?.AccessToken); // TODO replace
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

                            responseString += $"?new_user=true";
                        }
                        else
                        {
                            responseString += $"?new_user=false";
                        }

                        responseString += $"&success=true";
                        responseString += $"&access_token={tokenGoogle.AccessToken}";
                        responseString += $"&user_id={account.CommonId}";
                    }
                    else
                    {
                        responseString += "?success=false";
                        responseString += $"&exception=User info data were not correctly retrieved from provider: {provider}";
                    }
                }
                else
                {
                    responseString += "?success=false";
                    responseString += "&exception=An unexpected error happend during retrieving user info";
                }
            }

            if (provider == AuthProviders.Facebook)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFacebook?.AccessToken);
                HttpResponseMessage userInfoResponse = await httpClient.GetAsync($"https://graph.facebook.com/me?fields=id,name,email,picture,phone&access_token={tokenFacebook?.AccessToken}");

                if (userInfoResponse.IsSuccessStatusCode)
                {
                    string userInfoResString = await userInfoResponse.Content.ReadAsStringAsync();

                    if (!userInfoResString.IsNullOrEmpty())
                    {
                        FacebookUserInfo userInfo = JsonConvert.DeserializeObject<FacebookUserInfo>(userInfoResString);

                        // User not present in DB yet
                        Account? account = await _dataContext.Account.FirstOrDefaultAsync(account => account.CommonId == userInfo.Id);

                        if (account == null)
                        {
                            account = new Account
                            {
                                CommonId = userInfo?.Id,
                                Email = userInfo.Email,
                                IsCompany = false, // Pre-set that account is not company
                            };

                            _dataContext.Account.Add(account);
                            await _dataContext.SaveChangesAsync();

                            responseString += $"?new_user=true";
                        }
                        else
                        {
                            responseString += $"?new_user=false";
                        }

                        responseString += $"&success=true";
                        responseString += $"&access_token={tokenFacebook.AccessToken}";
                        responseString += $"&user_id={account.CommonId}";
                    }
                    else
                    {
                        responseString += "?success=false";
                        responseString += $"&exception=User info data were not correctly retrieved from provider: {provider}";
                    }
                }
                else
                {
                    responseString += "?success=false";
                    responseString += "&exception=An unexpected error happend during retrieving user info";
                }
            }

        }
        catch (Exception ex)
        {
            responseString += "?success=false";
            responseString += $"&exception={ex.Message}";
        }

        return responseString;
    }
}
