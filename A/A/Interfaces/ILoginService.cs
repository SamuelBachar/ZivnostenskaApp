using ExceptionsHandling;
using SharedTypesLibrary.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Interfaces;

public interface ILoginService
{
    Task<(UserLoginGenericResponse? UserInfo, ExceptionHandler? Exception)> LoginGeneric(string email, string passWord);
    Task<(UserOAuthResponse? UserInfo, ExceptionHandler? Exception)> LoginWithAuthProvider(string provider);
}
