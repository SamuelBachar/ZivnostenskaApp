﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs.Response;

public class UserLoginGenericResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;

    public bool RegisteredAsCustomer { get; set; }
    public bool RegisteredAsCompany { get; set; }

    public string JWT { get; set; } = string.Empty;

    public string JWTRefreshToken { get; set; } = string.Empty;
}
