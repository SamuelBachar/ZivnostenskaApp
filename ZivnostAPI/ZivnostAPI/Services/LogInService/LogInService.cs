using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Data.DataContext;

namespace ZivnostAPI.Services.LogInService;

public class LogInService : ILogInService
{
    private readonly DataContext _dataContext;

    public LogInService()
    {
        
    }

    public async Task<ApiResponse<UserLoginAuthProviderResponse?>> LogInWithAuthProvider(UserLoginAuthProviderRequest request)
    {
        ApiResponse<UserLoginAuthProviderResponse> response = new ApiResponse<UserLoginAuthProviderResponse>();

        return response;
    }
}
