using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedTypesLibrary.Constants;
using SharedTypesLibrary.Models.AuthProvidersData.Google;
using SharedTypesLibrary.Models.OAuthRefreshTokenRequest;
using SharedTypesLibrary.ServiceResponseModel;
using System.Net.Http;
using System.Net;
using ZivnostAPI.Models.AuthProvidersData;
using SharedTypesLibrary.Models.AuthProvidersData.Apple;
using SharedTypesLibrary.Models.AuthProvidersData.Facebook;
using ExtensionsLibrary.Http;
using ExtensionsLibrary.JsonExtensions;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Azure;

namespace ZivnostAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OAuthController : ControllerBase
{

    IHttpClientFactory _httpClientFactory;
    OAuthSettings _oAuthSettings;

    public OAuthController(IHttpClientFactory httpClientFactory, OAuthSettings oAuthSettings)
    {
        _httpClientFactory = httpClientFactory;
        _oAuthSettings = oAuthSettings;
    }

    [HttpPost("RefreshAccessToken")]
    public async Task<ActionResult<ApiResponse<OAuthRefreshTokenResponse>>> RefreshAccessToken([FromBody] OAuthRefreshTokenRequest request)
    {
        ObjectResult result;
        ApiResponse<OAuthRefreshTokenResponse> apiResponse;

        HttpClient? httpClient = null;
        string endPoint = string.Empty;
        Dictionary<string, string> tokenRequestData = new Dictionary<string, string>();

        if (request.Provider == AuthProviders.Google)
        {
            tokenRequestData = new Dictionary<string, string>
            {
                { "client_id", _oAuthSettings.Google.ClientId },        
                { "client_secret", _oAuthSettings.Google.ClientSecret },
                { "refresh_token", request.RefreshToken },
                { "grant_type", "refresh_token" }
            };

            httpClient = _httpClientFactory.CreateClient(AuthProviders.Google);

            endPoint = "https://oauth2.googleapis.com/token";
        }

        if (request.Provider == AuthProviders.Apple)
        {
            tokenRequestData = new Dictionary<string, string>
            {
                { "client_id", _oAuthSettings.Apple.ClientId },
                { "client_secret", _oAuthSettings.Apple.ClientSecret },
                { "refresh_token", request.RefreshToken },
                { "grant_type", "refresh_token" }
            };

            httpClient = _httpClientFactory.CreateClient(AuthProviders.Apple);
            endPoint = $"{_oAuthSettings.Apple.BaseUrl}/token";
        }

        if (httpClient != null)
        {
            FormUrlEncodedContent requestContent = new FormUrlEncodedContent(tokenRequestData);
            HttpResponseMessage tokenResponse = await httpClient.PostAsync(endPoint, requestContent);
            string responseString = await tokenResponse.Content.ExtReadAsStringAsync();

            if (request.Provider == AuthProviders.Google)
            {
                GoogleTokenResponse token = responseString.ExtJsonDeserializeObject<GoogleTokenResponse>();
                OAuthRefreshTokenResponse tokenResp = new OAuthRefreshTokenResponse 
                { 
                    NewAccessToken = token.AccessToken, 
                    NewRefreshToken = token.RefreshToken,
                    ExpiresIn = token.ExpiresIn
                };

                apiResponse = new ApiResponse<OAuthRefreshTokenResponse>(data: tokenResp);
                result = Ok(apiResponse);
            }
            else if (request.Provider == AuthProviders.Apple)
            {
                AppleTokenResponse token = responseString.ExtJsonDeserializeObject<AppleTokenResponse>();

                if (string.IsNullOrEmpty(token.Error))
                {
                    OAuthRefreshTokenResponse tokenResp = new OAuthRefreshTokenResponse 
                    { 
                        NewAccessToken = token.AccessToken, 
                        NewRefreshToken = token.RefreshToken ,
                        ExpiresIn = token.ExpiresIn
                    };

                    apiResponse = new ApiResponse<OAuthRefreshTokenResponse>(data: tokenResp);
                    result = Ok(apiResponse);
                }
                else
                {
                    apiResponse = new ApiResponse<OAuthRefreshTokenResponse>(data: null, success: false, apiErrorCode: "UAE_717", exceptionMessage: token.Error ?? "");
                    result = BadRequest(apiResponse);
                }
            }
            else
            {
                apiResponse = new ApiResponse<OAuthRefreshTokenResponse>(data: null, success: false, apiErrorCode: "UAE_713");
                result = BadRequest(apiResponse);
            }
        }
        else
        {
            apiResponse = new ApiResponse<OAuthRefreshTokenResponse>(data: null, success: false, apiErrorCode: "UAE_713");
            result = BadRequest(apiResponse);
        }

        return result;
    }
}
