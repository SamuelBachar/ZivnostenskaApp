using A.Enumerations;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Region = SharedTypesLibrary.DTOs.Bidirectional.Localization.Region;

namespace A.CustomControls.ApiEndpoints;

public static class ApiConfig
{

    private static readonly Dictionary<Type, Dictionary<ApiAction, string>> ApiEndpoints = new Dictionary<Type, Dictionary<ApiAction, string>>()
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

    public static string GetEndpoint<T>(ApiAction action)
    {
        var type = typeof(T);
        if (ApiEndpoints.TryGetValue(type, out var actions) && actions.TryGetValue(action, out var endpoint))
        {
            return endpoint;
        }

        throw new InvalidOperationException($"No API endpoint defined for {type.Name} with action {action}");
    }

}
