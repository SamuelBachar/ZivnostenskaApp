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
    private static FilterGroupManager _instance;
    public static FilterGroupManager Instance => _instance ??= new FilterGroupManager();

    private IRelationshipResolver? _relationshipResolver = null;

    private readonly Dictionary<string, List<object>> _filterGroups = new Dictionary<string, List<object>>();

    public FilterGroupManager() { }

    public void Initialize(IRelationshipResolver relationshipResolver)
    {
        if (relationshipResolver == null)
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

    public void NotifyPickerChanged<T>(CustomPicker<T> parentPicker, T parentSelectedItem)
    {
        var filterGroup = parentPicker.FilterGroup;

        if (_filterGroups.ContainsKey(filterGroup))
        {
            foreach (var pickerObj in _filterGroups[filterGroup])
            {
                if (pickerObj is CustomPicker<T> picker && picker != parentPicker)
                {
                    picker.FilterBy(itemChild => _relationshipResolver.AreRelated(parentSelectedItem, itemChild));
                }
            }
        }
    }
}
