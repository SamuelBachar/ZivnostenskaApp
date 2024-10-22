using CustomUIControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomControlsLibrary.Controls;

namespace CustomUIControls.Generics;

public class FilterGroupManager
{
    private static FilterGroupManager? _instance = null;
    public static FilterGroupManager Instance => _instance ??= new FilterGroupManager();

    private IRelationshipResolver? _relationshipResolver;

    private readonly Dictionary<string, List<object>> _filterGroups = new Dictionary<string, List<object>>();

    public FilterGroupManager() { }

    public void Initialize(IRelationshipResolver relationshipResolver)
    {
        if (_relationshipResolver == null)
        {
            _relationshipResolver = relationshipResolver;
        }
    }

    public void RegisterFilterAbleControl(IFilterable control, string filterGroup)
    {
        if (!_filterGroups.ContainsKey(filterGroup))
        {
            _filterGroups[filterGroup] = new List<object>();
        }

        _filterGroups[filterGroup].Add(control);
    }

    public void NotifyFilterAbleControlChange<TParent>(IFilterable parentControl, TParent parentItem)
    {
        var filterGroup = parentControl.FilterGroup;

        if (_filterGroups.ContainsKey(filterGroup))
        {
            foreach (var childControlObj in _filterGroups[filterGroup])
            {
                if (childControlObj is IFilterable childControl && !ReferenceEquals(childControl, parentControl))
                {
                    var filter = _relationshipResolver.AreRelated(parentItem, childControl.DataModelType);

                    if (filter != null)
                    {
                        childControl.FilterBy(filter, isFilteredByParentControl: true);
                    }
                }
            }
        }
    }
}
