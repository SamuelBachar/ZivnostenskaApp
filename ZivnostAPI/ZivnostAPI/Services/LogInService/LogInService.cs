using Azure;
using Azure.Core;
using ExceptionsHandling;
using ExtensionsLibrary.DbExtensions;
using ExtensionsLibrary.Http;
using ExtensionsLibrary.JsonExtensions;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using SharedTypesLibrary.Constants;
using SharedTypesLibrary.DbResponse;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.Models.AuthProvidersData.Apple;
using SharedTypesLibrary.Models.AuthProvidersData.Facebook;
using SharedTypesLibrary.Models.AuthProvidersData.Google;
using SharedTypesLibrary.ServiceResponseModel;
using System;
using System.Collections.Specialized;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using ZivnostAPI.Data.CusDbContext;
using ZivnostAPI.Models.AuthProvidersData;
using ZivnostAPI.Models.DatabaseModels.Account;
using static System.Net.WebRequestMethods;

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
                    response.ApiErrorCode = "UAE_713";
                }
            }
            else
            {
                response.Success = false;
                response.ApiErrorCode = "UAE_713";
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
                    { "client_id", $"{oauthSettings.Google.ClientId}"},
                    { "client_secret", $"{oauthSettings.Google.ClientSecret}"},
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
                    { "code", code },
                    { "client_id", oauthSettings.Facebook.ClientId},
                    { "client_secret", oauthSettings.Facebook.ClientSecret},
                    { "redirect_uri", oauthSettings.RedirectUri },
                    { "grant_type", "authorization_code" }
                };

                endPoint = "https://graph.facebook.com/v12.0/oauth/access_token";
            }
            else if (provider == AuthProviders.Apple)
            {
                httpClient = _httpClientFactory.CreateClient(AuthProviders.Apple);

                tokenRequestData = new Dictionary<string, string>
                {
                    { "code", code },
                    { "client_id", oauthSettings.Apple.ClientId },
                    { "client_secret", oauthSettings.Apple.ClientSecret },
                    { "redirect_uri", oauthSettings.RedirectUri },
                    { "grant_type", "authorization_code" }
                };

                endPoint = $"{oauthSettings.Apple.BaseUrl}/token";
            }
            else
            {
                throw new ExceptionHandler("UAE_713");
            }

            FormUrlEncodedContent requestContent = new FormUrlEncodedContent(tokenRequestData);
            HttpResponseMessage tokenResponse = await httpClient.PostAsync(endPoint, requestContent);

            if (tokenResponse.IsSuccessStatusCode)
            {
                ApiResponse<OAuthUserInfo> userInfo = new ApiResponse<OAuthUserInfo>();

                string responseContent = await tokenResponse.Content.ExtReadAsStringAsync();
                object tokenData = provider switch
                {
                    AuthProviders.Google => responseContent.ExtDeserializeObject<GoogleTokenResponse>(),
                    AuthProviders.Facebook => responseContent.ExtDeserializeObject<FacebookTokenResponse>(),
                    AuthProviders.Apple => responseContent.ExtDeserializeObject<AppleTokenResponse>(),
                    _ => throw new ExceptionHandler("UAE_713")
                };

                if (provider == AuthProviders.Apple)
                {
                    if (string.IsNullOrEmpty(((AppleTokenResponse)tokenData).Error))
                    {
                        userInfo = await GetUserInfoFromOAuthProvider(httpClient, tokenData, provider);
                    }
                    else
                    {
                        userInfo.Success = false;
                        userInfo.ApiErrorCode = "UAE_716";
                        userInfo.APIException = ((AppleTokenResponse)tokenData).Error ?? "";
                    }
                }
                else
                {
                    userInfo = await GetUserInfoFromOAuthProvider(httpClient, tokenData, provider);
                }

                if (userInfo?.Data != null && userInfo.Success)
                {
                    Account? account = await OAuthGetUserAccount(userInfo.Data, provider);
                    bool newUser = false;

                    if (account == null)
                    {
                        var dbResult = await OAuthCreateUserAccount(userInfo.Data, account, provider);

                        if (dbResult.IsSucces)
                        {
                            newUser = true;
                            response.Success = true;

                            SetUserOAuthResponse(response.Data, account, tokenData, newUser);
                        }
                        else
                        {
                            response.Success = false;
                            response.ApiErrorCode = dbResult.ApiErrorCode;
                            response.APIException = dbResult.Exception;
                        }
                    }
                    else
                    {
                        SetUserOAuthResponse(response.Data, account, tokenData, newUser);
                    }
                }
                else
                {
                    response.Success = userInfo.Success;
                    response.ApiErrorCode = userInfo.ApiErrorCode;
                    response.APIException = userInfo.APIException;
                }
            }
            else
            {
                response.Success = false;
                response.ApiErrorCode = "UAE_711";
            }
        }
        else
        {
            response.Success = false;
            response.ApiErrorCode = "UAE_712";
        }

        return response;
    }

    public async Task<ApiResponse<OAuthUserInfo>> GetUserInfoFromOAuthProvider<T>(HttpClient httpClient, T oAuthTokenResponse, string provider)
    {
        ApiResponse<OAuthUserInfo> result = new ApiResponse<OAuthUserInfo>();
        result.Data = new OAuthUserInfo();

        try
        {
            if (provider == AuthProviders.Google || provider == AuthProviders.Facebook)
            {
                string accessToken = string.Empty;
                string userInfoUrl = string.Empty;

                if (oAuthTokenResponse is GoogleTokenResponse googleToken)
                {
                    accessToken = googleToken.AccessToken;
                    userInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
                }

                if (oAuthTokenResponse is FacebookTokenResponse facebookToken)
                {
                    accessToken = facebookToken.AccessToken;
                    userInfoUrl = "https://graph.facebook.com/me?fields=id,name,email,picture,phone";
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage userInfoResponse = await httpClient.GetAsync(userInfoUrl);

                if (userInfoResponse.IsSuccessStatusCode)
                {
                    string userInfoResString = await userInfoResponse.Content.ReadAsStringAsync();

                    if (!userInfoResString.IsNullOrEmpty())
                    {
                        if (provider == AuthProviders.Google)
                        {
                            GoogleUserInfo? userInfo = userInfoResString.ExtDeserializeObject<GoogleUserInfo>();
                            result.Data.GoogleUserInfo = userInfo;
                            result.Success = true;
                        }
                        else if (provider == AuthProviders.Facebook)
                        {
                            FacebookUserInfo? userInfo = userInfoResString.ExtDeserializeObject<FacebookUserInfo>();
                            result.Data.FacebookUserInfo = userInfo;
                            result.Success = true;
                        }
                        else
                        {
                            result.Success = false;
                            result.ApiErrorCode = "UAE_713";
                        }
                    }
                    else
                    {
                        result.Success = false;
                        result.ApiErrorCode = "UAE_714";
                    }
                }
                else
                {
                    result.Success = false;
                    result.ApiErrorCode = "UAE_715";
                }
            }
            else if (provider == AuthProviders.Apple)
            {
                if (oAuthTokenResponse is AppleTokenResponse appleToken)
                {
                    JwtSecurityToken jwtToken = DecodeAppleJwtToken(appleToken.IdToken);
                    AppleUserInfo? userInfo = GetAppleUserInfo(jwtToken);

                    result.Data.AppleUserInfo = userInfo;
                    result.Success = true;
                }
                else
                {
                    throw new ArgumentException("Invalid token type");
                }
            }
            else
            {
                throw new ArgumentException("Invalid provider");
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.APIException = ex.Message;
        }

        return result;
    }

    public async Task<Account?> OAuthGetUserAccount(OAuthUserInfo userInfo, string provider)
    {
        Func<Account, bool> predicate = provider switch
        {
            AuthProviders.Google => account => account.CommonId == userInfo.GoogleUserInfo?.Id,
            AuthProviders.Facebook => account => account.CommonId == userInfo.FacebookUserInfo?.Id,
            AuthProviders.Apple => account => account.CommonId == userInfo.AppleUserInfo?.Id,
            _ => account => false // Fallback in case the provider does not match
        };

        return await _dataContext.Account.AsQueryable().FirstOrDefaultAsync(account => predicate(account));
    }

    public async Task<DbActionResponse> OAuthCreateUserAccount(OAuthUserInfo userInfo, Account? account, string provider)
    {
        DbActionResponse result = new DbActionResponse();

        try
        {
            if (provider == AuthProviders.Google && userInfo.GoogleUserInfo != null)
            {
                account = new Account
                {
                    IsCompanyAccount = false, // Pre-set Company to false (updated after user choose app mode during registration)
                    IsCustomerAccount = false, // Pre-set Customer to false (updated after user choose app mode during registration)
                    CommonId = userInfo.GoogleUserInfo.Id,
                    Email = userInfo.GoogleUserInfo.Email,
                    Name = userInfo.GoogleUserInfo.Name,
                    SureName = userInfo.GoogleUserInfo.FamilyName,
                    PictureURL = userInfo.GoogleUserInfo.Picture,
                };
            }

            if (provider == AuthProviders.Facebook && userInfo.FacebookUserInfo != null)
            {
                account = new Account
                {
                    IsCompanyAccount = false, // Pre-set Company to false (updated after user choose app mode during registration)
                    IsCustomerAccount = false, // Pre-set Customer to false (updated after user choose app mode during registration)
                    CommonId = userInfo.FacebookUserInfo.Id,
                    Email = userInfo.FacebookUserInfo.Email,
                    Name = userInfo.FacebookUserInfo.Name,
                    MiddleName = userInfo.FacebookUserInfo.MiddleName,
                    SureName = userInfo.FacebookUserInfo.LastName,
                    PictureURL = userInfo.FacebookUserInfo.Picture.PictureData.Url
                };
            }

            if (provider == AuthProviders.Apple && userInfo.AppleUserInfo != null)
            {
                account = new Account
                {
                    IsCompanyAccount = false, // Pre-set Company to false (updated after user choose app mode during registration)
                    IsCustomerAccount = false, // Pre-set Customer to false (updated after user choose app mode during registration)
                    CommonId = userInfo.AppleUserInfo.Id,
                    Email = userInfo.AppleUserInfo.Email,
                    Name = userInfo.AppleUserInfo.Name,
                    SureName = userInfo.AppleUserInfo.LastName
                };
            }

            if (account != null)
            {
                _dataContext.Account.Add(account);
            }

            DbActionResponse dbResponse = await _dataContext.SaveChangesWithCheckAsync();

            if (dbResponse.IsSucces)
            {
                result.IsSucces = true;
            }
            else
            {
                result.IsSucces = false;
                result.ApiErrorCode = "UAE_710";
                result.Exception = dbResponse.Exception;
            }
        }
        catch (Exception ex)
        {
            result.IsSucces = false;
            result.Exception = ex.Message;
        }

        return result;
    }

    public void SetUserOAuthResponse(UserOAuthResponse oAuthResponse, Account account, object tokenData, bool newUser)
    {
        oAuthResponse.Id = account.Id;
        oAuthResponse.OAuthId = account.CommonId;
        oAuthResponse.Email = account.Email;
        oAuthResponse.Phone = account.Phone;
        oAuthResponse.PictureURL = account.PictureURL;
        oAuthResponse.SureName = account.SureName;
        oAuthResponse.MiddleName = account.MiddleName;
        oAuthResponse.SureName = account.SureName;
        oAuthResponse.NewUser = newUser;

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

        if (tokenData is AppleTokenResponse appleToken)
        {
            oAuthResponse.OAuthAccessToken = appleToken.AccessToken;
            oAuthResponse.OauthRefreshToken = appleToken.RefreshToken;
            oAuthResponse.OAuthExpiresIn = appleToken.ExpiresIn;
        }
    }

    public string SerializeUserOAuthResponse(UserOAuthResponse oAuthResponse)
    {
        NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString[OAuthUrlParamsResponse.Id] = HttpUtility.UrlEncode(oAuthResponse.Id.ToString());
        queryString[OAuthUrlParamsResponse.OAuthId] = HttpUtility.UrlEncode(oAuthResponse.OAuthId);
        queryString[OAuthUrlParamsResponse.Email] = HttpUtility.UrlEncode(oAuthResponse.Email);
        queryString[OAuthUrlParamsResponse.Phone] = HttpUtility.UrlEncode(oAuthResponse.Phone);
        queryString[OAuthUrlParamsResponse.PictureUrl] = HttpUtility.UrlEncode(oAuthResponse.PictureURL);
        queryString[OAuthUrlParamsResponse.Name] = HttpUtility.UrlEncode(oAuthResponse.Name);
        queryString[OAuthUrlParamsResponse.MiddleName] = HttpUtility.UrlEncode(oAuthResponse.MiddleName);
        queryString[OAuthUrlParamsResponse.SureName] = HttpUtility.UrlEncode(oAuthResponse.SureName);
        queryString[OAuthUrlParamsResponse.OAuthAccessToken] = HttpUtility.UrlEncode(oAuthResponse.OAuthAccessToken);
        queryString[OAuthUrlParamsResponse.OAuthRefreshToken] = HttpUtility.UrlEncode(oAuthResponse.OauthRefreshToken);
        queryString[OAuthUrlParamsResponse.OAuthExpiresIn] = HttpUtility.UrlEncode(oAuthResponse.OAuthExpiresIn.ToString());
        queryString[OAuthUrlParamsResponse.NewUser] = oAuthResponse.NewUser ? "true" : "false";  // boolean values don't need encoding

        return queryString.ToString() ?? "";
    }

    private JwtSecurityToken DecodeAppleJwtToken(string appleIdToken)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(appleIdToken);

        return jwtToken;
    }

    private AppleUserInfo GetAppleUserInfo(JwtSecurityToken jwtToken) 
    {
        string? id = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty;
        string? email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty;
        string? firstName = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty;
        string? lastName = jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? string.Empty;

        AppleUserInfo appleUserInfo = new AppleUserInfo
        {
            Id = id,
            Email = email,
            Name = firstName,
            LastName = lastName
        };

        return appleUserInfo;
    }
}
