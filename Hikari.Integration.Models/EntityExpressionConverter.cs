#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：Hikari.Integration.Models
* 项目描述 ：
* 类 名 称 ：EntityExpressionConverter
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
using System.Reflection;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Data;

namespace Hikari.Integration.Models
{
    /* ============================================================================== 
* 功能描述：EntityExpressionConverter 通过表达式树转换实体(datatable,datareader)
* 创 建 者：jinyu 
* 创建日期：2019 
* 更新时间 ：2019
* ==============================================================================*/

   public static class EntityExpressionConverter
    {
        private static ConcurrentDictionary<string, object> Cache = new ConcurrentDictionary<string, object>();
        public static Action<T, object> GetSetter<T>(PropertyInfo property)
        {
            Action<T, object> result = null;
            Type type = typeof(T);
            string key = type.AssemblyQualifiedName + "_set_" + property.Name;
            object v = null;
            if(Cache.TryGetValue(key,out v))
            {
                result = v as Action<T, object>;
            }
            else
            {
                ParameterExpression parameter = Expression.Parameter(type, "t");
                ParameterExpression value = Expression.Parameter(typeof(object), "propertyValue");
                MethodInfo setter = type.GetMethod("set_" + property.Name);
                MethodCallExpression call = Expression.Call(parameter, setter, Expression.Convert(value, property.PropertyType));
                var lambda = Expression.Lambda<Action<T, object>>(call, parameter, value);
                result = lambda.Compile();
                 Cache[key] = result;
            }
            
            return result;
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

        /// <summary>
        /// 表达式树转换实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToEntityList<T>(this DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            if (dt == null || dt.Rows.Count == 0)
            {
                return list;
            }
            var Properties = typeof(T).GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var mapColumns = CheckProperty(dt, Properties);
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                foreach (var prop in mapColumns)
                {
                  
                    GetSetter<T>(prop.Property)(t, dr[prop.ColumnName]);
                    
                }
                list.Add(t);
            }

            return list;
        }

        /// <summary>
        /// 表达式树转换实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objReader"></param>
        /// <returns></returns>
        public static List<T> ToEntityList<T>(this IDataReader objReader) where T : new()
        {
            List<T> list = new List<T>();
            var Properties = typeof(T).GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var mapColumns = CheckProperty(objReader, Properties);
            while (objReader.Read())
            {
                T t = new T();
                foreach (var prop in mapColumns)
                {
                    GetSetter<T>(prop.Property)(t, objReader[prop.ColumnName]);
                }
                list.Add(t);
            }
            return list;
        }
    }
}
