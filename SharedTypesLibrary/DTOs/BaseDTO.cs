using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedTypesLibrary.DTOs;

public abstract class BaseDTO
{
    [JsonIgnore]
    public abstract string DisplayName { get; }
}
