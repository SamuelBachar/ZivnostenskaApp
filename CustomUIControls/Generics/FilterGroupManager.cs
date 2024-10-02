using CustomUIControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomControlsLibrary.Controls;
using CustomControlsLibrary.Controls.CustomPicker;

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

    public void RegisterPicker<T>(CustomPicker<T> picker, string filterGroup)
    {
        if (!_filterGroups.ContainsKey(filterGroup))
        {
            _filterGroups[filterGroup] = new List<object>();
        }

        _filterGroups[filterGroup].Add(picker);
    }

    public void NotifyPickerChanged<TParent>(CustomPicker<TParent> parentPicker, TParent parentItem)
    {
        var filterGroup = parentPicker.FilterGroup;

        if (_filterGroups.ContainsKey(filterGroup))
        {
            foreach (var pickerObj in _filterGroups[filterGroup])
            {
                if (pickerObj is ICustomPicker childPicker && childPicker != parentPicker)
                {
                    var filter = _relationshipResolver.AreRelated(parentItem, childPicker.DataModelType);

                    if (filter != null)
                    {
                        childPicker.FilterBy(filter);
                    }
                }
            }
        }
    }
}
