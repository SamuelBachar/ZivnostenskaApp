using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Request;

public class RegisterGenericCredentials
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PasswordConfirmed { get; set; }
}
