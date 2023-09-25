using Hikari;
using Hikari.PropertyWrapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

internal  class HikariExtensionHelpers
{
    /// <summary>
    /// 缓存属性转换
    /// </summary>
    private static readonly ConcurrentDictionary<string, string> concurrent = new();

    /// <summary>
    /// 缓存DB类型数据
    /// </summary>
    private static readonly ConcurrentDictionary<string, List<string>> dbType = new();




   

    /// <summary>
    /// 赋值类型和值 20230924
    /// </summary>
    /// <param name="value">IDbDataParameter</param>
    /// <param name="dataType">数据类型</param>
    /// <param name="v">数据值</param>
    /// <returns></returns>
    public static bool SetParameter(IDbDataParameter value,string dataType,object v)
    {
       
        string name = value.GetType().Name.Replace("Parameter", "");
        //赋值
        string dbp = name + "DbType";
        string dbv = name + "Value";
        if (v.GetType().Name == "String" && dataType != "String")
        {
            if (Enum.TryParse(typeof(TypeCode), dataType, out var result))
            {
               v= Convert.ChangeType(v,(TypeCode) result);
            }
        }
        
        var dbtype = value.GetType().GetProperty(dbp);
        var dbvalue = value.GetType().GetProperty(dbv);
        PropertyValue<IDbDataParameter> property = new(value);
        if (dbtype != null)
        {
            List<string> lst;
            if (!dbType.TryGetValue(dbp,out  lst))
            {
              lst = Enum.GetNames(dbtype.PropertyType).ToList();
                dbType[dbp]=lst;
            }
            if (lst.Contains(dataType))
            {
                object tmp = Enum.Parse(dbtype.PropertyType, dataType, true);
                property.Set(dbp, tmp);
            }
            else if (dataType == "String" && lst.Contains("Text"))
            {
                object tmp = Enum.Parse(dbtype.PropertyType, "Text", true);
                property.Set(dbp, tmp);
            }
        }
        //配置的特殊类型
        if (GlobalDBType.dicParameterType.TryGetValue
           (name, out var parameters))
        {
            if (parameters.TryGetValue(dataType, out string valueString))
            {
                if (dbtype != null)
                {
                    object tmp = Enum.Parse(dbtype.PropertyType, valueString, true);
                    property.Set(dbp, tmp);
                }
            }

        }
        if (dbvalue != null)
        {
            property.Set(dbv, v);
        }

        return false;
    }

    /// <summary>
    /// 转换SqlValue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ovj"></param>
    /// <returns></returns>
    internal static SqlValue ConvertSqlValue<T>(T ovj) where T : class
    {
        Type type = typeof(T);
        SqlValue sqlValue = new SqlValue();
        PropertyValue<T> propertyValue = new PropertyValue<T>(ovj);
        sqlValue.Value = propertyValue.Get("Value");
        string dty = "";
        if (!concurrent.TryGetValue(type.FullName, out dty))
        {
            dty = Check<T>(ovj);
        }
        sqlValue.Type = propertyValue.Get(dty).ToString();
        return sqlValue;

    }

    /// <summary>
    /// 获取属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ovj"></param>
    /// <returns></returns>
    internal static string Check<T>(T ovj)
    {
       
        Type type = typeof(T);
        var lst = type.GetProperties().ToList();
        foreach (var prop in lst)
        {
            if (prop.Name != "Value")
            {
                concurrent[type.FullName] = prop.Name;
                return prop.Name;
            }
        }
        return "";
    }
}