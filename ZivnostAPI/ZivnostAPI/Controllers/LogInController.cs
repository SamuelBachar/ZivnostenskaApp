﻿using Azure.Core;
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

    [HttpPost("LogInGeneric")]
    public async Task<ActionResult<ApiResponse<UserLoginGenericResponse>>> LogInGeneric(UserLoginGenericRequest request)
    {
        ObjectResult result;
        ApiResponse<UserLoginGenericResponse> response = await _loginService.LogInGeneric(request);

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

        ApiResponse<UserOAuthResponse> response;

        // todo could be this checked also for google or apple ?
        if (queryParams["state"].ToString() == AuthProviders.Facebook && !queryParams["error_description"].ToString().IsNullOrEmpty())
        {
            redirectUri += "success=false";

            if (!queryParams["error_description"].ToString().IsNullOrEmpty())
            {
                redirectUri += $"&exception={queryParams["error_description"]} - {queryParams["error_reason"]}";
            }
        }
        else
        {
            response = await _loginService.RedirectUri(queryParams);

            if (response.Success && response.Data != null)
            {
                redirectUri += "success=true";
                redirectUri += "&" + _loginService.SerializeUserOAuthResponse(response.Data);
            }
            else
            {
                redirectUri += "success=false";
                redirectUri += $"&exception={response.APIException}&apiErrorCode={response.ApiErrorCode}";
            }
        }

        return Redirect(redirectUri);
    }

}
