using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using SharedTypesLibrary.ServiceResponseModel;
using ZivnostAPI.Services.RegisterService;

namespace ZivnostAPI.Controllers;

[Route("api/RegisterController")]
[ApiController]
public class RegisterController : ControllerBase
{
    private readonly IRegisterService _registerService;
    public RegisterController(IRegisterService registerService)
    {
        _registerService = registerService;
    }

    [HttpPost("RegisterCompany")]
    public async Task<ActionResult<ApiResponse<RegistrationCompanyResponse>>> Register([FromForm] RegistrationCompanyRequest regCompData, [FromForm] IFormFile image)
    {
        ObjectResult result;
        ApiResponse<RegistrationCompanyResponse> response;

        response = await _registerService.RegisterCompany(regCompData, image);

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
}
