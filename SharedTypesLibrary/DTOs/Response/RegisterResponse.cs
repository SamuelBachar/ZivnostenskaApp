using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Response;

public class RegisterResponse
{
    public string Email { get; set; } = string.Empty;

    public DateTime DTConfirmRegisterUntil { get; set; }
}
