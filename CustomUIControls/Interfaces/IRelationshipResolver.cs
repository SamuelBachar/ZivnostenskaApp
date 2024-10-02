using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomUIControls.Interfaces;

public interface IRelationshipResolver
{
    public Func<object, bool>? AreRelated<TParent>(TParent parentItem, Type child);
    bool AreRelated<TParent, TChild>(TParent parentItem, TChild child);

    bool AreRelated(object parent, object child);
}
