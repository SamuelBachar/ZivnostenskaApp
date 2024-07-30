using A.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Services;
public partial class HttpClientService : IPlatformHttpMessageHandler
{
    public partial HttpMessageHandler GetPlatformSpecificHttpMessageHandler()
    {
        return null;
    }
}
