﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypesLibrary.Models.AuthProvidersData.Apple;

public class AppleUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
