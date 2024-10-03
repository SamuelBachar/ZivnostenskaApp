using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Models.AuthProvidersData.Facebook;
using ZivnostAPI.Models.AuthProvidersData.Google;

namespace ZivnostAPI.Services.LogInService;

public interface ILogInService
{
    Task<ApiResponse<OAuthLandingPageResponse?>> GetAuthProviderLandingPage(UserLoginAuthProviderLandingPageRequest request);

    Task<string> RedirectUri(IQueryCollection query);

    Task<string> GetUserInfoFromOAuthProvider(HttpClient httpClient, GoogleTokenResponse? tokenGoogle, FacebookTokenResponse? tokenFacebook, string provider, string responseString);
}
