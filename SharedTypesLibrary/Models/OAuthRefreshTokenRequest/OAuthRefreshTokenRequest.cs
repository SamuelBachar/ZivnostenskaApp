using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.Models.OAuthRefreshTokenRequest;

public class OAuthRefreshTokenRequest
{
    public string Provider { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
