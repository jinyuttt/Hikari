#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：Hikari.Integration.Models
* 项目描述 ：扩展数据库查询
* 类 名 称 ：DataReaderExtension
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
using System.Reflection;
using System.Text;

namespace Hikari.Integration.Models
{
    /* ============================================================================== 
* 功能描述：DataReaderExtension DataReader 转 Model
* 创 建 者：jinyu 
* 创建日期：2019 
* 更新时间 ：2019
* ==============================================================================*/

  public static  class DataReaderExtension
    {
        /// <summary>
        /// DataReader转泛型
        /// </summary>
        /// <typeparam name="T">传入的实体类</typeparam>
        /// <param name="objReader">DataReader对象</param>
        /// <returns></returns>
        public static IList<T> ReaderToList<T>(this IDataReader objReader) where T:new()
        {
            using (objReader)
            {
                List<T> list = new List<T>();

                //获取传入的数据类型
                var Properties = typeof(T).GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                var mapColumns = CheckProperty(objReader, Properties);

                //遍历DataReader对象
                while (objReader.Read())
                {
                    //使用与指定参数匹配最高的构造函数，来创建指定类型的实例
                    T model = new T();
                    foreach(var column  in mapColumns)
                    {
                        if(!IsNullOrDBNull(objReader[column.ColumnName]))
                        {
                            column.Property.SetValue(model, CheckType(objReader[column.ColumnName], column.Property.PropertyType), null);
                        }

                        
                    }
                    //for (int i = 0; i < objReader.FieldCount; i++)
                    //{
                    //    //判断字段值是否为空或不存在的值
                    //    if (!IsNullOrDBNull(objReader[i]))
                    //    {
                    //        //匹配字段名
                    //        PropertyInfo pi = modelType.GetProperty(objReader.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    //        if (pi != null)
                    //        {
                    //            //绑定实体对象中同名的字段  
                    //            pi.SetValue(model, CheckType(objReader[i], pi.PropertyType), null);
                    //        }
                    //    }
                    //}
                    list.Add(model);
                }
                return list;
            }
        }

        private static MapColumn[] CheckProperty(IDataReader reader, PropertyInfo[] Properties)
        {
            List<MapColumn> lst = new List<MapColumn>(Properties.Length);
            List<string> lstCol = new List<string>(reader.FieldCount); 
            for(int i=0;i<reader.FieldCount;i++)
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
                if(lstCol.Contains(colName))
                {
                    MapColumn column = new MapColumn() { ColumnName = colName, Property = property };
                    lst.Add(column);
                }
               
            }
            return lst.ToArray();
        }

        /// <summary>
        /// 对可空类型进行判断转换(*要不然会报错)
        /// </summary>
        /// <param name="value">DataReader字段的值</param>
        /// <param name="conversionType">该字段的类型</param>
        /// <returns></returns>
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

        /// <summary>
        /// 判断指定对象是否是有效值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsNullOrDBNull(object obj)
        {
            return (obj == null || (obj is DBNull)) ? true : false;
        }


        /// <summary>
        /// DataReader转模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objReader"></param>
        /// <returns></returns>
        public static T ReaderToModel<T>(this IDataReader objReader)
        {

            using (objReader)
            {
                if (objReader.Read())
                {
                    Type modelType = typeof(T);
                    int count = objReader.FieldCount;
                    T model = Activator.CreateInstance<T>();
                    for (int i = 0; i < count; i++)
                    {
                        if (!IsNullOrDBNull(objReader[i]))
                        {
                            PropertyInfo pi = modelType.GetProperty(objReader.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (pi != null)
                            {
                                pi.SetValue(model, CheckType(objReader[i], pi.PropertyType), null);
                            }
                        }
                    }
                    return model;
                }
            }
            return default(T);
        }
    }
}
