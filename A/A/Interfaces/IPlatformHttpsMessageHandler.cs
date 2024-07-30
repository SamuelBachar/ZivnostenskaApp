using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A.Interfaces;

public interface IPlatformHttpMessageHandler
{
    public HttpMessageHandler GetPlatformSpecificHttpMessageHandler();
}
