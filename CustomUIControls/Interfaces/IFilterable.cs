using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CustomUIControls.Interfaces;


public interface IFilterable<T>
{
    string FilterGroup { get; }
    Type DataModelType { get; }
    void FilterBy(Func<object, bool> filter);
}
