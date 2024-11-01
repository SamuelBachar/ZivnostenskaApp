using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedTypesLibrary.DTOs.Bidirectional.Localization;

namespace A.CustomControls.CustomControlsDefines.RelationshipDefines;

public static class FilterRelations
{
    public static readonly Dictionary<Type, Dictionary<Type, Delegate>> Relations = new Dictionary<Type, Dictionary<Type, Delegate>>()
    {
        {
            typeof(CountryDTO), new Dictionary<Type, Delegate>
            {
                { typeof(RegionDTO), new Func<object, object, bool>((country, region) => ((CountryDTO)country).Id == ((RegionDTO)region).Country_Id) }
            }
        },
        {
            typeof(RegionDTO), new Dictionary<Type, Delegate>
            {
                { typeof(DistrictDTO), new Func<object, object, bool>((region, district) => ((RegionDTO)region).Id == ((DistrictDTO)district).Region_Id) }
            }
        },
        {
            typeof(DistrictDTO), new Dictionary<Type, Delegate>
            {
                { typeof(CityDTO), new Func<object, object, bool>((district, city) => ((DistrictDTO)district).Id == ((CityDTO)city).District_Id) }
            }
        }
    };
}