using Hikari;
using System;
using System.Collections.Generic;
using System.Data;

internal  class HikariExtensionHelpers
{
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
}