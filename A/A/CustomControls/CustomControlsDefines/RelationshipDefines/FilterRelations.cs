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
            typeof(RegionDTO), new Dictionary<Type, Delegate>
            {
                { typeof(DistrictDTO), new Func<RegionDTO, DistrictDTO, bool>((region, district) => district.Region_Id == region.Id) }
            }
        }
    };
}