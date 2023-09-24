using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hikari.PropertyWrapper
{
    public class SetterWrapper<TTarget, TValue> : ISetValue
    {
        private Action<TTarget, TValue> _setter;

        public SetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");

            MethodInfo m = propertyInfo.GetSetMethod(true);
            _setter = (Action<TTarget, TValue>)Delegate.CreateDelegate(typeof(Action<TTarget, TValue>), null, m);
        }

        public void SetValue(TTarget target, TValue val)
        {
            _setter(target, val);
        }
        void ISetValue.Set(object target, object val)
        {
            _setter((TTarget)target, (TValue)val);
        }
    }
}
