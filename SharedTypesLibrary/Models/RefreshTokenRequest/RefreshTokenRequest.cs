﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.Models.RefreshTokenRequest;

public class RefreshTokenRequest
{
    public string Provider { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}