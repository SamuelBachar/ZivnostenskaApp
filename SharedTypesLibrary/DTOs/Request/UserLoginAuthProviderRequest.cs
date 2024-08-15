using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Request
{
    public class UserLoginAuthProviderRequest
    {
        public string Provider { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
