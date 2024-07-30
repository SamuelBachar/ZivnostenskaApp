using A.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Services;
public partial class HttpClientService : IPlatformHttpMessageHandler // TODO: to be tested
{
    public partial HttpMessageHandler GetPlatformSpecificHttpMessageHandler()
    {
        var handler = new NSUrlSessionHandler
        {
            TrustOverrideForUrl = (nsUrlSessionHandler, url, secTrust) =>
            {
                if (url.Contains("https://localhost"))
                {
                    return true;
                }

                if (url.Contains("azure"))
                {
                    return true;
                }

                return false;
            }
        };

        return handler;
    }
}
