using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;

namespace ZivnostAPI.Services.RegisterService;

public interface IRegisterService
{
    Task<ApiResponse<RegistrationCompanyResponse>> RegisterCompany(RegistrationCompanyRequest request, IFormFile image);
}
