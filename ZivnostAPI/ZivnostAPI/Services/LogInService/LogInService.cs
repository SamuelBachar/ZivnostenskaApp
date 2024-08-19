using Azure;
using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.Constants;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Data.DataContext;

namespace ZivnostAPI.Services.LogInService;

public class LogInService : ILogInService
{
    private readonly DataContext _dataContext;
    private readonly IHttpClientFactory _httpClientFactory;

    public LogInService(IHttpClientFactory httpClientFactory, DataContext dataContext)
    {
        _httpClientFactory = httpClientFactory;
        _dataContext = dataContext;
    }

    public async Task<ApiResponse<UserLoginAuthProviderResponse?>> GetAuthProviderLandingPage(UserLoginAuthProviderRequest request)
    {
        ApiResponse<UserLoginAuthProviderResponse?> response = new ApiResponse<UserLoginAuthProviderResponse?>();

        if (request.Provider == AuthProviders.Google)
        {
            HttpClient client = _httpClientFactory.CreateClient(AuthProviders.Google);

            HttpResponseMessage googleResponse = await client.GetAsync(string.Empty);

            // Check if the response is successful
            if (googleResponse.IsSuccessStatusCode)
            {
                // Read the response content as a string
                var responseData = await googleResponse.Content.ReadAsStringAsync();

                // You can deserialize the response if it's JSON, for example:
                // var userInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(responseData);

                // For demonstration, just set the raw response data to your ApiResponse
                response.Data = new UserLoginAuthProviderResponse
                {
                    WebPage = client.BaseAddress.ToString()//responseData
                };

                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.ExceptionMessage = $"Error from Google API: {googleResponse.ReasonPhrase}";
            }
        }
        else
        {
            response.Success = false;
            response.ExceptionMessage = "Unsupported provider";
        }

        return response;
    }

    public async Task<ApiResponse<UserLoginAuthProviderResponse?>> AuthenticateWithAuthProvider(UserLoginAuthProviderRequest request)
    {
        ApiResponse<UserLoginAuthProviderResponse?> response = new ApiResponse<UserLoginAuthProviderResponse?>();

        if (request.Provider == AuthProviders.Google)
        {
            var client = _httpClientFactory.CreateClient();
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");

            var tokenRequestData = new Dictionary<string, string>
            {
                { "code", request.Code },
                { "client_id", "YOUR_GOOGLE_CLIENT_ID" },
                { "client_secret", "YOUR_GOOGLE_CLIENT_SECRET" },
                { "redirect_uri", "YOUR_REDIRECT_URI" },
                { "grant_type", "authorization_code" }
            };

            tokenRequest.Content = new FormUrlEncodedContent(tokenRequestData);
            var tokenResponse = await client.SendAsync(tokenRequest);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenResponseData = await tokenResponse.Content.ReadAsStringAsync();
                // Process token, save user, etc.
                response.Success = true;
                response.Data = new UserLoginAuthProviderResponse { Token = tokenResponseData };
            }
            else
            {
                response.Success = false;
                response.Data = null;
            }
        }
        else
        {
            response.Success = false;
            response.Data = null;
        }

        return response;
    }

    public void RedirectUri()
    {
        int a;
        int b =+ 2;
    }

}
