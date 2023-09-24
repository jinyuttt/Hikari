using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hikari.PropertyWrapper
{
    internal interface ISetValue
    {
        void Set(object target, object val);
    }
}
