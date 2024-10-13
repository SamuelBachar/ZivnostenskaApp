using A.Models.OAuthTokenData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Interfaces;

public interface IOauthService
{
    Task ReloadUserDataFromOAuthProvider(ITokenData tokenData);
}
