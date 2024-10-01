using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomControlsLibrary.Controls.CustomPicker;

interface ICustomPicker
{
    public void FilterBy(Func<object, bool> filter);
}
