#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：Hikari.Integration.Models
* 项目描述 ：
* 类 名 称 ：EntityConverter
* 类 描 述 ：
* 命名空间 ：Hikari.Integration.Models
* CLR 版本 ：4.0.30319.42000
* 作    者 ：jinyu
* 创建时间 ：2019
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ jinyu 2019. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion



using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Concurrent;

namespace Hikari.Integration.Models.Emit
{
    /* ============================================================================== 
* 功能描述：EntityConverter Emit转换实体
* 创 建 者：jinyu 
* 创建日期：2019 
* 更新时间 ：2019
* ==============================================================================*/

    public static class EntityEmitConverter
    {
        private static ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>();
        //数据类型和对应的强制转换方法的methodinfo，供实体属性赋值时调用
        private static Dictionary<Type, MethodInfo> ConvertMethods = new Dictionary<Type, MethodInfo>()
       {
           {typeof(int),typeof(Convert).GetMethod("ToInt32",new Type[]{typeof(object)})},
           {typeof(Int16),typeof(Convert).GetMethod("ToInt16",new Type[]{typeof(object)})},
           {typeof(Int64),typeof(Convert).GetMethod("ToInt64",new Type[]{typeof(object)})},
           {typeof(DateTime),typeof(Convert).GetMethod("ToDateTime",new Type[]{typeof(object)})},
           {typeof(decimal),typeof(Convert).GetMethod("ToDecimal",new Type[]{typeof(object)})},
           {typeof(Double),typeof(Convert).GetMethod("ToDouble",new Type[]{typeof(object)})},
           {typeof(Boolean),typeof(Convert).GetMethod("ToBoolean",new Type[]{typeof(object)})},
           {typeof(string),typeof(Convert).GetMethod("ToString",new Type[]{typeof(object)})}
          
       };

        //把datarow转换为实体的方法的委托定义
        public delegate T LoadDataRow<T>(DataRow dr);
        //把datareader转换为实体的方法的委托定义
        public delegate T LoadDataRecord<T>(IDataRecord dr);

        //emit里面用到的针对datarow的元数据信息
        private static readonly AssembleInfo dataRowAssembly = new AssembleInfo(typeof(DataRow));
        //emit里面用到的针对datareader的元数据信息
        private static readonly AssembleInfo dataRecordAssembly = new AssembleInfo(typeof(IDataRecord));

        /// <summary>
        /// 构造转换动态方法（核心代码），根据assembly可处理datarow和datareader两种转换
        /// </summary>
        /// <typeparam name="T">返回的实体类型</typeparam>
        /// <param name="assembly">待转换数据的元数据信息</param>
        /// <returns>实体对象</returns>
        private static DynamicMethod BuildMethod<T>(AssembleInfo assembly, MapColumn[] mapColumns = null)
        {
            DynamicMethod method = new DynamicMethod(assembly.MethodName + typeof(T).Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(T),
                    new Type[] { assembly.SourceType }, typeof(EntityContext).Module, true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(T));
            generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            foreach (var column in  mapColumns)
             //   foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                PropertyInfo property = column.Property;
                var endIfLabel = generator.DefineLabel();
                generator.Emit(OpCodes.Ldarg_0);
                //第一组，调用AssembleInfo的CanSetted方法，判断是否可以转换
                //  generator.Emit(OpCodes.Ldstr, property.Name);
                generator.Emit(OpCodes.Ldstr, column.ColumnName);
                generator.Emit(OpCodes.Call, assembly.CanSettedMethod);
                generator.Emit(OpCodes.Brfalse, endIfLabel);
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ldarg_0);
            
                //第二组,属性设置
                generator.Emit(OpCodes.Ldstr, column.ColumnName);
                generator.Emit(OpCodes.Call, assembly.GetValueMethod);//获取数据库值
                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                    generator.Emit(OpCodes.Call, ConvertMethods[property.PropertyType]);//调用强转方法赋值
                    //效果类似  Name=Convert.ToString(row["PName"]);
                else
                    generator.Emit(OpCodes.Castclass, property.PropertyType);
                generator.Emit(OpCodes.Call, property.GetSetMethod());//直接给属性赋值
                //效果类似  Name=row["PName"];
                generator.MarkLabel(endIfLabel);
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            return method;
        }

        /// <summary>
        /// 从Cache获取委托 LoadDataRow<T>的方法实例，没有则调用BuildMethod构造一个。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static LoadDataRow<T> GetDataRowMethod<T>(MapColumn[] mapColumns=null)
        {
            string key = dataRowAssembly.MethodName + typeof(T).FullName;
            LoadDataRow<T> load = null;
            object v = null;
            if (cache.TryGetValue(key, out v))
            {
                load = v as LoadDataRow<T>;
            }
            else
            {
                load = (LoadDataRow<T>)BuildMethod<T>(dataRowAssembly,mapColumns).CreateDelegate(typeof(LoadDataRow<T>));
                cache[key] = load;
            }

            return load;
        }

        /// <summary>
        /// 从Cache获取委托 LoadDataRecord<T>的方法实例，没有则调用BuildMethod构造一个。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static LoadDataRecord<T> GetDataRecordMethod<T>(MapColumn[] mapColumns)
        {
            string key = dataRecordAssembly.MethodName + typeof(T).Name;
            LoadDataRecord<T> load = null;
            object v = null;
            if (cache.TryGetValue(key, out v))
            {
                load = v as LoadDataRecord<T>;
            }
            else
            {
                load = (LoadDataRecord<T>)BuildMethod<T>(dataRecordAssembly,mapColumns).CreateDelegate(typeof(LoadDataRecord<T>));
                cache[key] = load;
            }

            return load;
        }


        public static T ToItem<T>(DataRow dr)
        {
            LoadDataRow<T> load = GetDataRowMethod<T>();
            return load(dr);
        }

        /// <summary>
        /// EMIT 转换实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToEntityEmitList<T>(this DataTable dt)
        {
            List<T> list = new List<T>();
            if (dt == null || dt.Rows.Count == 0)
            {
                return list;
            }
            var Properties = typeof(T).GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var mapColumns = CheckProperty(dt, Properties);
            LoadDataRow<T> load = GetDataRowMethod<T>(mapColumns);
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(load(dr));
            }
            return list;
        }

        /// <summary>
        /// EMIT转换实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static List<T> ToEntityEmitList<T>(this IDataReader dr)
        {
            List<T> list = new List<T>();
            var Properties = typeof(T).GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var mapColumns = CheckProperty(dr, Properties);
            LoadDataRecord<T> load = GetDataRecordMethod<T>(mapColumns);
           
            while (dr.Read())
            {
                list.Add(load(dr));
            }
            return list;
        }

        /// <summary>
        /// 检查列
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Properties"></param>
        /// <returns></returns>
        private static MapColumn[] CheckProperty(DataTable dt, PropertyInfo[] Properties)
        {
            List<MapColumn> lst = new List<MapColumn>(Properties.Length);
            foreach (var property in Properties)
            {
                string colName = property.Name;
                DataFieldAttribute aliasAttr = property.GetCustomAttribute<DataFieldAttribute>();
                if (aliasAttr != null)
                {
                    colName = aliasAttr.ColumnName;
                }
                if (dt.Columns.Contains(colName))
                {
                    MapColumn column = new MapColumn() { ColumnName = colName, Property = property };
                    lst.Add(column);
                }
            }
            return lst.ToArray();

        }

        /// <summary>
        /// 检查可匹配的列
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="Properties"></param>
        /// <returns></returns>
        private static MapColumn[] CheckProperty(IDataReader reader, PropertyInfo[] Properties)
        {
            List<MapColumn> lst = new List<MapColumn>(Properties.Length);
            List<string> lstCol = new List<string>(reader.FieldCount);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                lstCol.Add(reader.GetName(i));
            }
            foreach (var property in Properties)
            {
                string colName = property.Name;
                DataFieldAttribute aliasAttr = property.GetCustomAttribute<DataFieldAttribute>();
                if (aliasAttr != null)
                {
                    colName = aliasAttr.ColumnName;
                }
                if (lstCol.Contains(colName))
                {
                    MapColumn column = new MapColumn() { ColumnName = colName, Property = property };
                    lst.Add(column);
                }

            }
            return lst.ToArray();
        }



    }
}
