using Azure.Core;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedTypesLibrary.Constants;
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
    public async Task<ActionResult<ApiResponse<OAuthLandingPageResponse>>> GetAuthProviderLandingPage(AuthProviderLandingPageRequest request)
    {
        ObjectResult result;
        ApiResponse<OAuthLandingPageResponse?> response = await _loginService.GetAuthProviderLandingPage(request);

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
        string redirectUri = AuthProviderCallBackDataSchemes.MobileCallBackDataScheme;

        if (queryParams["state"].ToString() == AuthProviders.Facebook && !queryParams["error_description"].ToString().IsNullOrEmpty()) // todo could be this checked also for google or apple ?
        {
            redirectUri += "success=false";

            if (!queryParams["error_description"].ToString().IsNullOrEmpty())
            {
                redirectUri += $"&exception={queryParams["error_description"]} - {queryParams["error_reason"]}";
            }
        }
        else
        {
            //redirectUri = await _loginService.RedirectUri(queryParams);
        }

        return Redirect(redirectUri);
    }

}
