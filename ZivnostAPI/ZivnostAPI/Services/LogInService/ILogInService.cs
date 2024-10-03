using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Models.AuthProvidersData;
using ZivnostAPI.Models.AuthProvidersData.Facebook;
using ZivnostAPI.Models.AuthProvidersData.Google;
using ZivnostAPI.Models.DatabaseModels.Account;

namespace ZivnostAPI.Services.LogInService;

public interface ILogInService
{
    Task<ApiResponse<OAuthLandingPageResponse?>> GetAuthProviderLandingPage(AuthProviderLandingPageRequest request);

    Task<ApiResponse<UserOAuthResponse>> RedirectUri(IQueryCollection query);

    Task<ApiResponse<OAuthUserInfo>> GetUserInfoFromOAuthProvider<T>(HttpClient httpClient, T oAuthTokenResponse, string provider);

    Task<Account> OAuthGetUserAccount(OAuthUserInfo userInfo, string provider);

    Task<bool> OAuthCreateUserAccount(OAuthUserInfo userInfo, Account? account);

    void UserOAuthResponse(UserOAuthResponse oAuthResponse, Account account, object tokenData);
}
