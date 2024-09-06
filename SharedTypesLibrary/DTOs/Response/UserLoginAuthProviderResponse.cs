using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Response;

public class UserLoginAuthProviderResponse
{

    public string Provider { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty; // Temporary

    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SureName { get; set; } = string.Empty;

    public string JWT { get; set; } = string.Empty; // Generic JWT handled by API
    public string RefreshToken { get; set; } = string.Empty; // Generic Refresh token handled by API

    public string OAuthUrl { get; set; } = string.Empty;

    public bool NewUser { get; set; } = false;
}
