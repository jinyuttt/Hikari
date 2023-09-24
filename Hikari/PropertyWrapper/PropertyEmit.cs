using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Concurrent;

namespace Hikari.PropertyWrapper
{
    internal class PropertyEmit
    {
        private static ConcurrentDictionary<PropertyInfo, SetValueDelegate> _memberDelegate = new();
        public delegate void SetValueDelegate(object target, object arg);

        /// <summary>
        /// 创建动态对象，减少装箱拆箱
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static SetValueDelegate CreatePropertySetter(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            if (!property.CanWrite)
            {
                return null;
            }

          var setter=  _memberDelegate.GetOrAdd(property, CreatePropertyDelegate);
            return setter;
        }

        private static SetValueDelegate CreatePropertyDelegate(PropertyInfo property)
        {
            MethodInfo setMethod = property.GetSetMethod(true);

            DynamicMethod dm = new DynamicMethod("PropertySetter", null,
                new Type[] { typeof(object), typeof(object) },
                property.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!setMethod.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            il.Emit(OpCodes.Ldarg_1);

            EmitCastToReference(il, property.PropertyType);
            if (!setMethod.IsStatic && !property.DeclaringType.IsValueType)
            {
                il.EmitCall(OpCodes.Callvirt, setMethod, null);
            }
            else
            {
                il.EmitCall(OpCodes.Call, setMethod, null);
            }
            il.Emit(OpCodes.Ret);

            return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
        }

        private static void EmitCastToReference(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }
    
}
}
