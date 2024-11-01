﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Models.OAuthTokenData;

public class AppleTokenData
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ValidUntil { get; set; }

    public string JwtToken { get; set; } = string.Empty;
}
