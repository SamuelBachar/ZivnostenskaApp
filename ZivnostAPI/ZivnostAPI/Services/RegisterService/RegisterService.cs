using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Data.CusDbContext;

namespace ZivnostAPI.Services.RegisterService;

public class RegisterService : IRegisterService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly CusDbContext _dataContext;

    public RegisterService(IHttpClientFactory httpClientFactory, CusDbContext dataContext)
    {
        _dataContext = dataContext;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ApiResponse<RegistrationCompanyResponse>> RegisterCompany(RegistrationCompanyRequest request, IFormFile image)
    {
        ApiResponse<RegistrationCompanyResponse> response = new ApiResponse<RegistrationCompanyResponse>();

        if (request == null)
        {
            response.Success = false;
        }
        else
        {
            // Process image if present
            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    request.Image = memoryStream.ToArray();
                }
            }

            // TODO
            // update account type
            // create Company
            // create record in CompanyImages

        }

        return response;
    }
}
