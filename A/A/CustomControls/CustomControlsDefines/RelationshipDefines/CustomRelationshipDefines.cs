using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomUIControls.Interfaces;

using Region = SharedTypesLibrary.DTOs.Bidirectional.Localization.RegionDTO;
using System.Reflection.Metadata.Ecma335;

namespace A.CustomControls.CustomControlsDefines.RelationshipDefines;

public class CustomRelationshipDefines : IRelationshipResolver
{
    public Func<object, bool>? AreRelated<TParent>(TParent parentItem, Type childType)
    {
        Func<object, bool>? result = null;

        var parentType = typeof(TParent);

        if (CheckIfRelationExist(parentType, childType))
        {
            var relationFunc = FilterRelations.Relations[parentType][childType] as Func<object, object, bool>;

            if (relationFunc != null)
            {
                result = childItem => relationFunc(parentItem, childItem);
            }
        }

        return result;
    }

    public bool AreRelated<TParent, TChild>(TParent parentItem, TChild childItem)
    {
        return AreRelated((object)parentItem, (object)childItem);
    }

    public bool AreRelated(object parentItem, object childItem)
    {
        bool result = false;

        var parentType = parentItem.GetType();
        var childType = childItem.GetType();

        if (CheckIfRelationExist(parentType, childType))
        {
            var relationFunc = FilterRelations.Relations[parentType][childType] as Func<object, object, bool>;

            if (relationFunc != null)
            {
                result = relationFunc(parentItem, childItem);
            }
        }
        else
        {
            result = true;
        }

        return result;
    }

    private bool CheckIfRelationExist(Type parentType, Type childType)
    {
        return FilterRelations.Relations.ContainsKey(parentType) && FilterRelations.Relations[parentType].ContainsKey(childType);
    }
}
