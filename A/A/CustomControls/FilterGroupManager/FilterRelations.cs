using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Region = SharedTypesLibrary.DTOs.Bidirectional.Localization.Region;

namespace A.CustomControls.FilterGroupManager;

public static class FilterRelations
{
    public static readonly Dictionary<Type, Dictionary<Type, Delegate>> Relations = new Dictionary<Type, Dictionary<Type, Delegate>>()
    {
        {
            typeof(Region), new Dictionary<Type, Delegate>
            {
                { typeof(District), new Func<Region, District, bool>((region, district) => district.Region_Id == region.Id) }
            }
        }
    };
}
