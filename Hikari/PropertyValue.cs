using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Hikari
{

    /// <summary>
    ///处理属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyValue<T>
    {
        private static ConcurrentDictionary<string, MemberGetDelegate> _memberGetDelegate = new ConcurrentDictionary<string, MemberGetDelegate>();
      //  private static ConcurrentDictionary<string, MemberSetDelegate> memberSetDelegate = new ConcurrentDictionary<string, MemberSetDelegate>();
        delegate object MemberGetDelegate(T obj);
        delegate object MemberSetDelegate(T obj,object v);
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

        public void Set(string name,object obj)
        {
            BuildSetDelegate(name);
          //  MemberSetDelegate memberGet = memberSetDelegate.GetOrAdd(name, BuildSetDelegate);
          //  memberGet(Target,obj);
        }
        private MemberGetDelegate BuildDelegate(string name)
        {
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(name);
            return (MemberGetDelegate)Delegate.CreateDelegate(typeof(MemberGetDelegate), property.GetGetMethod());

        }

        private MemberSetDelegate BuildSetDelegate(string name)
        {
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(name);
          
           // Delegate.CreateDelegate(typeof(Action<S>),Target, property.GetSetMethod());
            return (MemberSetDelegate)Delegate.CreateDelegate(typeof(MemberSetDelegate),Target,property.GetSetMethod());

        }

      
    
    }
   
}
