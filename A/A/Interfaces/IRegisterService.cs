using ExceptionsHandling;
using SharedTypesLibrary.DTOs.Request;
using SharedTypesLibrary.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Interfaces;

public interface IRegisterService
{
    Task<(RegisterResponse? RegisterInfo, ExceptionHandler? Exception)> RegisterCompany(RegistrationCompanyRequest request);
    Task<(RegisterResponse? RegisterInfo, ExceptionHandler? Exception)> RegisterUser(RegisterUserRequest request);
}
