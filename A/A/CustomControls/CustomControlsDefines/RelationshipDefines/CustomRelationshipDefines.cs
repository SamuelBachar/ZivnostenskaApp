using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomUIControls.Interfaces;

using Region = SharedTypesLibrary.DTOs.Bidirectional.Localization.Region;


namespace A.CustomControls.CustomControlsDefines.RelationshipDefines;

public class CustomRelationshipDefines : IRelationshipResolver
{
    public bool AreRelated<TParent, TChild>(TParent parentItem, TChild childItem)
    {
        var parentType = typeof(TParent);
        var childType = typeof(TChild);

        if (FilterRelations.Relations.ContainsKey(parentType) && FilterRelations.Relations[parentType].ContainsKey(childType))
        {
            // Retrieve the relationship function and invoke it with the provided items
            var relationFunc = FilterRelations.Relations[parentType][childType] as Func<TParent, TChild, bool>;

            if (relationFunc != null)
            {
                return relationFunc(parentItem, childItem);
            }
        }

        // Default to true if no specific relation is defined (e.g., unrelated types are allowed)
        return true;
    }
}
