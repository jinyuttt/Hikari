using System;
using System.Collections.Concurrent;
using System.Reflection;
using static Hikari.PropertyWrapper.PropertyEmit;

namespace Hikari.PropertyWrapper
{

    /// <summary>
    ///处理属性
    /// </summary>
    /// <typeparam name="T">实例类型</typeparam>
    public class PropertyValue<T>
    {
        private static ConcurrentDictionary<string, MemberGetDelegate> _memberGetDelegate = new ConcurrentDictionary<string, MemberGetDelegate>();
          private static ConcurrentDictionary<string, SetValueDelegate> memberSetDelegate = new ConcurrentDictionary<string, SetValueDelegate>();
        delegate object MemberGetDelegate(T obj);
    
        public PropertyValue(T obj)
        {
            Target = obj;
        }
        public T Target { get; private set; }
        public object Get(string name)
        {
            MemberGetDelegate memberGet = _memberGetDelegate.GetOrAdd(name, BuildDelegate);
            return memberGet(Target);
        }

        public void Set<S>(string name, S obj)
        {
            SetValueDelegate valueDelegate= memberSetDelegate.GetOrAdd(name, BuildSetDelegate);
            valueDelegate(Target, obj);

        }
        private MemberGetDelegate BuildDelegate(string name)
        {
           
            PropertyInfo property = Target.GetType().GetProperty(name);
            return (MemberGetDelegate)Delegate.CreateDelegate(typeof(MemberGetDelegate), property.GetGetMethod());

        }

        /// <summary>
        /// 创建属性设置委托
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private SetValueDelegate BuildSetDelegate(string name)
        {
           
            PropertyInfo property = Target.GetType().GetProperty(name);
            if(property == null)
            {
                throw new InvalidOperationException(string.Format("没有属性{0}",name));
            }
            //属性设置用EMIT，委托方式存在大量装箱拆箱，性能会降低很多。
            return PropertyEmit.CreatePropertySetter(property);
        }
        


    }

}
