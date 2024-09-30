using A.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedTypesLibrary.DTOs.Bidirectional.Localization;

using CustomUIControls.Interfaces;
using static CustomUIControls.Enumerations.Enums;


namespace A.CustomControls.CustomControlsDefines.EndpointDefines;

public class CustomEndpointDefines : IEndpointResolver
{

    private static readonly Dictionary<Type, Dictionary<ApiAction, string>> Endpoints = new Dictionary<Type, Dictionary<ApiAction, string>>()
    {
        {
            typeof(RegionDTO), new Dictionary<ApiAction, string>()
            {
                { ApiAction.GetAll, $"/api/RegionController/GetAll" },
                { ApiAction.GetById,$"/api/RegionController/GetById/{{id}}" }
            }
        },
        {
            typeof(DistrictDTO), new Dictionary<ApiAction, string>()
            {
                { ApiAction.GetAll, $"/api/DistrictController/GetAll" },
                { ApiAction.GetById, $"/api/DistrictController/GetById/{{id}}" }
            }
        }
    };

    public string GetEndpoint<T>(ApiAction action)
    {
        var type = typeof(T);
        if (Endpoints.TryGetValue(type, out var actions) && actions.TryGetValue(action, out var endpoint))
        {
            return endpoint;
        }

        throw new InvalidOperationException($"No API endpoint defined for {type.Name} with action {action}");
    }

}
