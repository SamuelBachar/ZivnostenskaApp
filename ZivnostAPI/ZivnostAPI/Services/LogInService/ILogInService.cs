using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Models.CompanyBaseData;

namespace ZivnostAPI.Services.LogInService;

public interface ILogInService
{
    Task<ApiResponse<UserLoginAuthProviderResponse?>> LogInWithAuthProvider(UserLoginAuthProviderRequest request);
}
