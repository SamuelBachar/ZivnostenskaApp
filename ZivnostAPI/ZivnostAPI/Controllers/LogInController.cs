using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Services.LogInService;

namespace ZivnostAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogInController : ControllerBase
{
    ILogInService _loginService;
    public LogInController(ILogInService loginService)
    {

        _loginService = loginService;

    }

    [HttpPost("GetAuthProviderLandingPage")]
    public async Task<ActionResult<ApiResponse<UserLoginAuthProviderResponse>>> GetAuthProviderLandingPage(UserLoginAuthProviderRequest request)
    {
        ObjectResult result;
        ApiResponse<UserLoginAuthProviderResponse?> response = await _loginService.GetAuthProviderLandingPage(request);

        if (response.Success)
        {
            result = Ok(response);
        }
        else
        {
            result = BadRequest(response);
        }

        return result;
    }

    [HttpPost("AuthenticateWithAuthProvider")]
    public async Task<ActionResult<ApiResponse<UserLoginAuthProviderResponse>>> AuthenticateWithAuthProvider(UserLoginAuthProviderRequest request)
    {
        ObjectResult result;
        ApiResponse<UserLoginAuthProviderResponse?> response = await _loginService.AuthenticateWithAuthProvider(request);

        if (response.Success)
        {
            result = Ok(response);
        }
        else
        {
            result = BadRequest(response);
        }

        return result;
    }

    [HttpGet("RedirectUri")]
    public ActionResult RedirectUri()
    {
        // Get the query parameters
        var queryParams = HttpContext.Request.Query;

        // Extract only the parameters you're interested in
        var code = queryParams["code"].ToString();
        var state = queryParams["state"].ToString();

        // You can add checks for the presence of these parameters
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Authorization code is missing");
        }

        // Use the parameters for your logic
        // For example, redirecting to the app with the code
        var redirectUri = $"myapp://auth?code={code}&state={state}";

        return Redirect(redirectUri);
    }

}
