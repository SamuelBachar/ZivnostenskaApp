using A.Exceptions.UserActionException;
using SharedTypesLibrary.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Interfaces;

public interface ILoginService
{
    Task<(UserLoginDataDTO UserInfo, ExceptionHandler? Exception)> LoginHTTPS(string email, string passWord);
}
