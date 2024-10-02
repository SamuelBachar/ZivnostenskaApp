using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Request;

public class RegisterUserRequest
{
    RegisterGenericCredentials? RegGenericData { get; set; }

    RegisterAuthProviderCredentials? RegAuthProviderData { get; set; }

    public string Name { get; set; } = string.Empty;
    public string SureName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
