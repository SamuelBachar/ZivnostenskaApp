using A.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Services;
public partial class HttpClientService
{
    public HttpClient GetPlatformSpecificHttpClient()
    {
        var httpMessageHandler = GetPlatformSpecificHttpMessageHandler();
        return new HttpClient(httpMessageHandler);
    }

    public partial HttpMessageHandler GetPlatformSpecificHttpMessageHandler();
    // https://www.youtube.com/watch?v=-Wj1JYkgWNU&ab_channel=AbhayPrince
}
