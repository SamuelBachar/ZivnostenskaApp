using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CustomUIControls.Interfaces;


public interface IFilterable<T>
{
    void FilterBy(Func<T, bool> filterPredicate);
}
