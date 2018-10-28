using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

/**
* 命名空间: Hikari 
* 类 名： ProxyLoad
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace Hikari
{
    /// <summary>
    /// 功能描述    ：ProxyLoad  加载驱动
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/24 22:49:18 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/24 22:49:18 
    /// </summary>
   public class ProxyLoad
    {
        private static readonly string DllName = ".dll";
        private static object lock_obj = new object();
        private static Dictionary<string, Type> dic_ConnectType = new Dictionary<string, Type>();
        private static Dictionary<string, AssemblyDLLType> dicAssemblyDLLType = new Dictionary<string, AssemblyDLLType>();
        public static IDbConnection GetConnection(string path,string clazz)
        {
            Type type = null;
            if (dic_ConnectType.TryGetValue(clazz,out type))
            {
               
                IDbConnection connection = (IDbConnection)Activator.CreateInstance(type);
                return connection;
            }
            else
            {
                if(path==null)
                {
                    return null;
                }
                if(!path.ToLower().Trim().EndsWith(DllName))
                {
                    path = path + DllName;
                }
                try
                {
                    Assembly assembly = Assembly.LoadFrom(path); //利用dll的路径加载,同时将此程序集所依赖的程序集加载进来,需后辍名.dll
                    Type[] allTypes = assembly.GetTypes();
                    string cls = clazz.Trim().ToLower();
                    foreach (Type tmp in allTypes)
                    {
                        if (tmp.Name.ToLower() == cls || tmp.FullName.ToLower() == cls)
                        {
                            type = tmp;
                            break;
                        }
                    }
                    if (type == null)
                    {
                        Logger.Singleton.ErrorFormat("{0}-程序集，类型{1}没有找到", path, clazz);
                    }
                    dic_ConnectType[clazz] = type;
                    IDbConnection connection = (IDbConnection)Activator.CreateInstance(type);
                    return connection;
                }
                catch(Exception ex)
                {
                    Logger.Singleton.Error(string.Format("{0}-程序集加载失败",path), ex);
                }
                return null;
            }
        }

        /// <summary>
        /// 获取驱动对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IDbConnection GetConnection(string path)
        {
            Type type = null;
            AssemblyDLLType assemblyDLLType = null;
            if (dicAssemblyDLLType.TryGetValue(path, out assemblyDLLType))
            {
                IDbConnection connection = (IDbConnection)Activator.CreateInstance(assemblyDLLType.ConnectType);
                return connection;
            }
            else
            {
                if (path == null)
                {
                    return null;
                }
                string dllPath = path;
                if (!path.ToLower().Trim().EndsWith(DllName))
                {
                    dllPath = path + DllName;
                }
                bool token = false;
                try
                {
                   
                    Monitor.Enter(lock_obj,ref token);
                    FileInfo fileInfo = new FileInfo(dllPath);
                    if(!fileInfo.Exists)
                    {
                        Logger.Singleton.Error("没有找到驱动程序集！路径："+path);
                    }
                    Assembly assembly = Assembly.LoadFrom(dllPath); //利用dll的路径加载,同时将此程序集所依赖的程序集加载进来,需后辍名.dll
                    Type[] allTypes = assembly.GetTypes();
                    AssemblyDLLType dLLType = new AssemblyDLLType();
                    int num = 4;
                    foreach (Type tmp in allTypes)
                    {
                        if(!tmp.IsClass||tmp.IsAbstract)
                        {
                            continue;
                        }
                        if(typeof(IDbConnection).IsAssignableFrom(tmp))
                        {
                            type = tmp;
                            dLLType.ConnectType = tmp;
                            num--;
                        }
                        if (typeof(IDbCommand).IsAssignableFrom(tmp))
                        {
                          
                            dLLType.CommandType = tmp;
                            num--;
                        }
                        if (typeof(IDbDataAdapter).IsAssignableFrom(tmp))
                        {
                           
                            dLLType.DataAdapterType = tmp;
                            num--;
                        }
                        if (typeof(IDbDataParameter).IsAssignableFrom(tmp))
                        {

                            dLLType.ParameterType = tmp;
                            num--;
                        }
                        if(num==0)
                        {
                            break;
                        }
                    }
                    if (type == null)
                    {
                        Logger.Singleton.ErrorFormat("{0}-程序集，类型没有找到", path);
                    }
                    dicAssemblyDLLType[path] = dLLType;
                    IDbConnection connection = (IDbConnection)Activator.CreateInstance(type);
                    return connection;
                }
                catch (Exception ex)
                {
                    Logger.Singleton.Error(string.Format("{0}-程序集加载失败", path), ex);
                }
                finally
                {
                    if (token) Monitor.Exit(lock_obj);
                }
                return null;
            }
        }

        /// <summary>
        /// 创建无参构造函数对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IDbDataParameter GetDataParameter(string path)
        {
            AssemblyDLLType assemblyDLLType = null;
            if (dicAssemblyDLLType.TryGetValue(path, out assemblyDLLType))
            {
                IDbDataParameter parameter = (IDbDataParameter)Activator.CreateInstance(assemblyDLLType.ParameterType);
                return parameter;
            }
            return null;
        }

        /// <summary>
        /// 创建无参构造函数对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IDbDataAdapter GetDataAdapter(string path)
        {
            AssemblyDLLType assemblyDLLType = null;
            if (dicAssemblyDLLType.TryGetValue(path, out assemblyDLLType))
            {
                IDbDataAdapter adapter = (IDbDataAdapter)Activator.CreateInstance(assemblyDLLType.DataAdapterType);
                return adapter;
            }
            return null;
        }

        /// <summary>
        /// 创建无参构造函数对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IDbCommand GetDbCommand(string path)
        {
            AssemblyDLLType assemblyDLLType = null;
            if (dicAssemblyDLLType.TryGetValue(path, out assemblyDLLType))
            {
                IDbCommand command = (IDbCommand)Activator.CreateInstance(assemblyDLLType.CommandType);
                return command;
            }
            return null;
        }
    }
}
