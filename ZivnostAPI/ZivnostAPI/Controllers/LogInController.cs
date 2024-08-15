using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.Request;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Services.LogInService;

namespace ZivnostAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogInController : Controller
{
    ILogInService _loginService;
    public LogInController(ILogInService loginService)
    {

        _loginService = loginService;

    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<UserLoginAuthProviderResponse>>> LogInWithAuthProvider(UserLoginAuthProviderRequest request)
    {
        ApiResponse<UserLoginAuthProviderResponse> response = await _loginService.LogInWithAuthProvider(request);

        return Ok(response);
    }

}
