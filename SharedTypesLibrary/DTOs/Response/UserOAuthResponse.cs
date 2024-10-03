using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Response;

public class UserOAuthResponse
{
    string Id { get; set; } = string.Empty;

    public string Provider { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    // JWT Used for all clients regardless of authentication method
    public string JWT { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;

    // Used against further login via OAuth providers if not expired yet
    public string AccessToken { get; set; } = string.Empty;

    public int ExpiresIn { get; set; }

    public bool NewUser { get; set; } = false;
}
