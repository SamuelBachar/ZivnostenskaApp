using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Response;

public class UserLoginAuthProviderResponse
{
    string Email { get; set; } = string.Empty;

    string Token { get; set; } = string.Empty;

    string JWT { get; set; } = string.Empty;
}
