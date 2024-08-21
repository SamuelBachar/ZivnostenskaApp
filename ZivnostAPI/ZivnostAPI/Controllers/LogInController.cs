using Azure.Core;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using System.Net.Http;
using ZivnostAPI.Models.AuthProvidersData;
using ZivnostAPI.Services.LogInService;

namespace ZivnostAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogInController : ControllerBase
{
    ILogInService _loginService;
    IServiceProvider _serviceProvider;
    public LogInController(ILogInService loginService, IServiceProvider serviceProvider)
    {

        _loginService = loginService;
        _serviceProvider = serviceProvider;
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
    public async Task<ActionResult<string>> RedirectUri()
    {
        IQueryCollection queryParams = HttpContext.Request.Query;
        string redirectUri = await _loginService.RedirectUri(queryParams);

        return Redirect(redirectUri);
    }

}
