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

    public async Task<ApiResponse<UserLoginAuthProviderResponse?>> LogInWithAuthProvider(UserLoginAuthProviderRequest request)
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
                    Token = responseData
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

}
