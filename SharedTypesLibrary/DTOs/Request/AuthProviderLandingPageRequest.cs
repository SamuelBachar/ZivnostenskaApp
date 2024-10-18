using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Request
{
    public class AuthProviderLandingPageRequest
    {
        public string Provider { get; set; } = string.Empty;
        public bool IsFirstLogin { get; set; }
    }
}
