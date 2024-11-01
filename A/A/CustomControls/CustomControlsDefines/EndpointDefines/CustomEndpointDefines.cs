using A.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedTypesLibrary.DTOs.Bidirectional.Localization;

using CustomUIControls.Interfaces;
using static CustomUIControls.Enumerations.Enums;
using SharedTypesLibrary.DTOs.Bidirectional.Services;
using SharedTypesLibrary.DTOs.Bidirectional.Categories;


namespace A.CustomControls.CustomControlsDefines.EndpointDefines;

public class CustomEndpointDefines : IEndpointResolver
{

    private static readonly Dictionary<Type, Dictionary<ApiAction, string>> Endpoints = new Dictionary<Type, Dictionary<ApiAction, string>>()
    {
        {
            typeof(CountryDTO), new Dictionary<ApiAction, string>()
            {
                { ApiAction.GetAll, $"/api/CountryController/GetAll" },
                { ApiAction.GetById,$"/api/CountryController/GetById/{{id}}" }
            }
        },
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
        },
        {
            typeof(CityDTO), new Dictionary<ApiAction, string>()
            {
                { ApiAction.GetAll, $"/api/CityController/GetAll" },
                { ApiAction.GetById, $"/api/CityController/GetById/{{id}}" }
            }
        },
        {
            typeof(ServiceDTO), new Dictionary<ApiAction, string>()
            {
                { ApiAction.GetAll, $"/api/ServiceController/GetAll" },
                { ApiAction.GetById, $"/api/ServiceController/GetById/{{id}}" }
            }
        },
        {
            typeof(CategoryDTO), new Dictionary<ApiAction, string>()
            {
                { ApiAction.GetAll, $"/api/CategoryController/GetAll" },
                { ApiAction.GetById, $"/api/CategoryController/GetById/{{id}}" }
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
