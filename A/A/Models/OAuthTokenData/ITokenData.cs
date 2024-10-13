using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Models.OAuthTokenData;

public interface ITokenData
{
    string AccessToken { get; set; }
    DateTime ValidUntil { get; set; }
}
