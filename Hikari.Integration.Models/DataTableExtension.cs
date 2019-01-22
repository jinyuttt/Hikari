#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：Hikari.Integration.Models
* 项目描述 ：
* 类 名 称 ：DataTableExtension
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
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hikari.Integration.Models
{
    /* ============================================================================== 
* 功能描述：DataTableExtension 
* 创 建 者：jinyu 
* 创建日期：2019 
* 更新时间 ：2019
* ==============================================================================*/

    public static class DataTableExtension
    {
        /// <summary>
        /// convert datatable into entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToModelList<T>(this DataTable dt) where T : new()
        {
            List<T> entity_list = new List<T>();
            var Properties = typeof(T).GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            Properties= CheckProperty(dt, Properties);
            foreach (DataRow row in dt.Rows)
            {
                ToList(entity_list, Properties, row);
            }
            return entity_list;
        }

        /// <summary>
        /// 检查列
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Properties"></param>
        /// <returns></returns>
        private static PropertyInfo[] CheckProperty(DataTable dt, PropertyInfo[] Properties)
        {
            List<PropertyInfo> lst = new List<PropertyInfo>(Properties.Length);
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
                    
                    lst.Add(property);
                }
            }
            return lst.ToArray();

        }

        /// <summary>
        /// convert List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity_list"></param>
        /// <param name="Properties"></param>
        /// <param name="row"></param>
        private static void ToList<T>(List<T> entity_list, PropertyInfo[] Properties, DataRow row) where T : new()
        {
            try
            {
                var entity = new T();
                foreach (var property in Properties)
                {
                    string colName = property.Name;
                    DataFieldAttribute aliasAttr = property.GetCustomAttribute<DataFieldAttribute>();
                    if (aliasAttr != null)
                    {
                        colName = aliasAttr.ColumnName;
                    }

                    if (row[colName] == DBNull.Value)
                    {
                        property.SetValue(entity, " ", null);
                      
                    }
                    else
                    {
                        property.SetValue(entity, CheckType(row[colName], property.PropertyType), null);
                      
                    }
                    entity_list.Add(entity);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}: \r\n{string.Join(",", row.ItemArray)}", ex);
            }
        }

        private static object CheckType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;
                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(value, conversionType);
        }


        public static object ToEntity(this DataTable dt, Type type)
        {
            MethodInfo[] methods = typeof(DataTableExtension)
                .GetMethods(BindingFlags.Public | BindingFlags.Static);

            MethodInfo method = methods
                .FirstOrDefault(m => m.Name == nameof(DataTableExtension.ToEntity) && m.ContainsGenericParameters == true)
                .MakeGenericMethod(type);
            return method.Invoke(null, new[] { dt });
        }
       
        
        /// <summary>
        /// convert list of entities into datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static DataTable FromEntity<T>(this List<T> entities)
        {
            DataTable dt = new DataTable();
            foreach (var property in typeof(T).GetProperties())
            {
                string colName = property.Name;
                //if enable below lines, DataTable will add Columns with Alias name
                //var aliasAttr = property.GetCustomAttributes().FirstOrDefault(a => a as AliasAttribute != null);
                //if (aliasAttr != null)
                //{
                //    colName = aliasAttr.GetType().GetProperty(nameof(AliasAttribute.ColumnName)).GetValue(aliasAttr) as string;
                //}

                //todo: Message: System.NotSupportedException : DataSet does not support System.Nullable<>.
                dt.Columns.Add(new DataColumn() { ColumnName = colName });
            }
            foreach (var entity in entities)
            {
                DataRow row = dt.Rows.Add(
                    (from p in typeof(T).GetProperties()
                     where dt.Columns.Contains(p.Name)
                     select p.GetValue(entity)).ToArray()
                     );

            }
            return dt;
        }
    
}
}
