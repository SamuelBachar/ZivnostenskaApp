using A.Models.OAuthTokenData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Models.OAuthLoginData;

class FacebookTokenData : ITokenData
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime ValidUntil { get; set; }
}
