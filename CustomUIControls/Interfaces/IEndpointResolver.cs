using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomUIControls.Enumerations.Enums;

namespace CustomUIControls.Interfaces;

public interface IEndpointResolver
{
    string GetEndpoint<T>(ApiAction action);
}
