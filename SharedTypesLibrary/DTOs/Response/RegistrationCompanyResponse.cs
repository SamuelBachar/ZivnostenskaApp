using SharedTypesLibrary.DTOs.Bidirectional.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Response;

public class RegistrationCompanyResponse
{
    public JwtDTO JwtData { get; set; }
}
