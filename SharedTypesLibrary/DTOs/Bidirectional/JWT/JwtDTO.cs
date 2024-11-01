using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Bidirectional.JWT;

public class JwtDTO
{
    public string JwtToken { get; set; } = string.Empty;

    public string JwtRefreshToken { get; set; } = string.Empty;

    DateTime TokenValidUntil {  get; set; }
}
