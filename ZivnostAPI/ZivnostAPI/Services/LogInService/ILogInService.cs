using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Models.AuthProvidersData.Facebook;
using ZivnostAPI.Models.AuthProvidersData.Google;
using ZivnostAPI.Models.CompanyBaseData;

namespace ZivnostAPI.Services.LogInService;

public interface ILogInService
{
    Task<ApiResponse<UserLoginAuthProviderResponse?>> GetAuthProviderLandingPage(UserLoginAuthProviderRequest request);

    Task<ApiResponse<UserLoginAuthProviderResponse?>> AuthenticateWithAuthProvider(UserLoginAuthProviderRequest request);

    Task<string> RedirectUri(IQueryCollection query);

    Task<string> ProcessUserAuthentication(HttpClient httpClient, GoogleTokenResponse? tokenGoogle, FacebookTokenResponse? tokenFacebook, string provider, string responseString);
}
