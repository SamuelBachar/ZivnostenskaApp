using ExceptionsHandling;
using SharedTypesLibrary.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static A.Enums.Enums;

namespace A.Interfaces;

public interface ILoginService
{
    Task<(UserLoginGenericResponse UserInfo, ExceptionHandler? Exception)> LoginGeneric(string email, string passWord);
    Task<(UserLoginAuthProviderResponse UserInfo, ExceptionHandler? Exception)> LoginWithAuthProvider(string provider);
}
