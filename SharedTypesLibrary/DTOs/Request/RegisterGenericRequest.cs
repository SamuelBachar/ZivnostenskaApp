using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Request;

public class RegisterGenericRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PasswordConfirmed { get; set; }
}
