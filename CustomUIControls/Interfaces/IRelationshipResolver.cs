using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomUIControls.Interfaces;

public interface IRelationshipResolver
{
    bool AreRelated<TParent, TChild>(TParent parent, TChild child);
}
