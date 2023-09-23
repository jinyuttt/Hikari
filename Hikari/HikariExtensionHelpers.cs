using Hikari;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

internal  class HikariExtensionHelpers
{
    private static ConcurrentDictionary<string, string> concurrent = new ConcurrentDictionary<string, string>();
    public static bool SetParameter(string sqlValue, IDbDataParameter value)
    {
        string name = value.GetType().Name.Replace("Parameter","");
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        if (GlobalDBType.dicParameterType.TryGetValue
            (name, out parameters))
        {
            if (parameters.TryGetValue(sqlValue, out string valueString))
            {
                //赋值
                var dbtype = value.GetType().GetProperty(name + "DbType");

                if (dbtype != null)
                {
                    Object v = Enum.Parse(dbtype.PropertyType, valueString,true);
                    dbtype.SetValue(value,v , null);
                }
            }

        }
        return false;
    }

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