using A.CustomControls.CustomPicker;
using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Region = SharedTypesLibrary.DTOs.Bidirectional.Localization.Region;
using District = SharedTypesLibrary.DTOs.Bidirectional.Localization.District;

namespace A.CustomControls.FilterGroupManager;

public class FilterGroupManager
{
    private static FilterGroupManager _instance;
    public static FilterGroupManager Instance => _instance ??= new FilterGroupManager();

    private Dictionary<string, List<object>> _filterGroups;

    private FilterGroupManager()
    {
        _filterGroups = new Dictionary<string, List<object>>();
    }

    // Register a picker in the specified group
    public void RegisterPicker<T>(CustomPicker<T> picker, string filterGroup)
    {
        if (!_filterGroups.ContainsKey(filterGroup))
        {
            _filterGroups[filterGroup] = new List<object>();
        }

        _filterGroups[filterGroup].Add(picker);
    }

    // Notify when a picker has changed in a filter group
    public void NotifyPickerChanged<T>(CustomPicker<T> sourcePicker, T selectedItem)
    {
        var filterGroup = sourcePicker.FilterGroup;

        if (_filterGroups.ContainsKey(filterGroup))
        {
            foreach (var pickerObj in _filterGroups[filterGroup])
            {
                if (pickerObj is CustomPicker<T> picker && picker != sourcePicker)
                {
                    // Assuming we know how to filter based on the parent-child relationship
                    picker.FilterBy(item => AreRelated(selectedItem, item));
                }
            }
        }
    }

    public static bool AreRelated<T1, T2>(T1 parentItem, T2 childItem)
    {
        var parentType = typeof(T1);
        var childType = typeof(T2);

        if (FilterRelations.Relations.ContainsKey(parentType) && FilterRelations.Relations[parentType].ContainsKey(childType))
        {
            // Retrieve the relationship function and invoke it with the provided items
            var relationFunc = FilterRelations.Relations[parentType][childType] as Func<T1, T2, bool>;

            if (relationFunc != null)
            {
                return relationFunc(parentItem, childItem);
            }
        }

        // Default to true if no specific relation is defined (e.g., unrelated types are allowed)
        return true;
    }
}

