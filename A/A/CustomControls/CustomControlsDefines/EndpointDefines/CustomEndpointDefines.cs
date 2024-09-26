using A.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomUIControls.Interfaces;

using Region = SharedTypesLibrary.DTOs.Bidirectional.Localization.Region;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using static CustomUIControls.Enumerations.Enums;


namespace A.CustomControls.CustomControlsDefines.EndpointDefines;

public class CustomEndpointDefines : IEndpointResolver
{

    private static readonly Dictionary<Type, Dictionary<ApiAction, string>> Endpoints = new Dictionary<Type, Dictionary<ApiAction, string>>()
    {
        {
            typeof(Region), new Dictionary<ApiAction, string>()
            {
                { ApiAction.GetAll, $"/api/{nameof(Region)}/GetAll" },
                { ApiAction.GetById,$"/api/{nameof(Region)}/GetById/{{id}}" }
            }
        },
        {
            typeof(District), new Dictionary<ApiAction, string>()
            {
                { ApiAction.GetAll, $"/api/{nameof(District)}/GetAll" },
                { ApiAction.GetById, $"/api/{nameof(District)}/GetById/{{id}}" }
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
