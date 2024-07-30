using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Response;

public class UserLoginDataDTO
{
    public string Email { get; set; } = string.Empty;

    public string JWT { get; set; } = string.Empty;
}
