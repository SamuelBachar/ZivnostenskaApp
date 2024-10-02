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
    Task<(RegisterGenericResponse? RegisterInfo, ExceptionHandler? Exception)> RegisterGeneric(RegisterGenericRequest request);
    Task<(UserLoginGenericResponse? UserInfo, ExceptionHandler? Exception)> RegisterCompanyGeneric(string email, string passWord);
}
