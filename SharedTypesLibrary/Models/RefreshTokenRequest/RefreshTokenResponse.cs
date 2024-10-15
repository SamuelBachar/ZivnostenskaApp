using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.Models.RefreshTokenRequest;

public class RefreshTokenResponse
{
    public string NewAccessToken { get; set; } = string.Empty;
    public string NewRefreshToken { get; set; } = string.Empty;

    public int ExpiresIn { get; set; }
}
