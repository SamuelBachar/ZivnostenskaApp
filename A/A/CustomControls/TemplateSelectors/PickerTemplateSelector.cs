using SharedTypesLibrary.DTOs.Bidirectional.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Region = SharedTypesLibrary.DTOs.Bidirectional.Localization.Region;

namespace A.CustomControls.TemplateSelectors;

public class PickerTemplateSelector : DataTemplateSelector
{
    public DataTemplate? _regionTemplate { get; set; }
    public DataTemplate? _districtTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        DataTemplate? dataTemplate = null;

        if (item is Region)
        {
            dataTemplate = _regionTemplate;
        }

        if (item is District)
        {
            dataTemplate = _districtTemplate;
        }

        return dataTemplate;
    }
}
