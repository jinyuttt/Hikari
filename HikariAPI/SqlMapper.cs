#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：HikariAPI
* 项目描述 ：
* 类 名 称 ：SqlMapper
* 类 描 述 ：
* 命名空间 ：HikariAPI
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
using Hikari.Integration.Models;
using Hikari.Manager;
using System.Data;

namespace HikariAPI
{
    /* ============================================================================== 
* 功能描述：SqlMapper 
* 创 建 者：jinyu 
* 创建日期：2019 
* 更新时间 ：2019
* ==============================================================================*/

    public class SqlMapper : IORM
    {
        private readonly string CfgName = null;
        public SqlMapper(string name=null)
        {
            CfgName = name;
           
        }
        //public int Execute<P>(string sql, P param = default(P))
        //{
        //    using (var con = ManagerPool.Singleton.GetDbConnection(CfgName))
        //    {
        //        var cmd = ManagerPool.Singleton.CreateDbCommand(CfgName);
        //        cmd.Connection = con;
        //        cmd.CommandText = sql;

        //        if (param != null)
        //        {
        //            foreach (var p in typeof(P).GetProperties())
        //            {
        //                IDataParameter parameter = ManagerPool.Singleton.CreateDataParameter(CfgName);
        //                parameter.Value = p.GetValue(param);
        //                parameter.ParameterName = "@" + p.Name;
        //                cmd.Parameters.Add(parameter);
        //            }
        //        }
        //       return cmd.ExecuteNonQuery();
        //    }
        //}

        public int Execute<P>(string sql, params dynamic[] param)
        {
            using (var con = ManagerPool.Singleton.GetDbConnection(CfgName))
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;

                if (param != null)
                {
                    int index = 0;
                    foreach (dynamic p in param)
                    {
                        if (p is ValueType)
                        {
                            object obj = p;
                            var parameter = cmd.CreateParameter();
                            parameter.Value = obj;
                            parameter.ParameterName = "@" + ValueTypeParam.ParamArray[index++];
                            cmd.Parameters.Add(parameter);
                        }
                        else if (p is object)
                        {
                            object obj = p;
                            var properties = obj.GetType().GetProperties();
                            foreach (var py in properties)
                            {
                                var parameter = cmd.CreateParameter();
                                parameter.Value = py.GetValue(obj);
                                parameter.ParameterName = "@" + py.Name;
                                cmd.Parameters.Add(parameter);

                            }
                        }
                    }
                }
                return cmd.ExecuteNonQuery();
            }
            
            }

        //public object ExecuteScalar<T, P>(string sql, P param = default(P))
        //{
        //    using (var con = ManagerPool.Singleton.GetDbConnection(CfgName))
        //    {
        //        var cmd = ManagerPool.Singleton.CreateDbCommand(CfgName);
        //        cmd.Connection = con;
        //        cmd.CommandText = sql;

        //        if (param != null)
        //        {
        //            foreach (var p in typeof(P).GetProperties())
        //            {
        //                IDataParameter parameter = ManagerPool.Singleton.CreateDataParameter(CfgName);
        //                parameter.Value = p.GetValue(param);
        //                parameter.ParameterName = "@" + p.Name;
        //                cmd.Parameters.Add(parameter);
        //            }
        //        }
        //        return cmd.ExecuteScalar();
        //    }
        //}

        public object ExecuteScalar<T>(string sql, params dynamic[] param)
        {
            using (var con = ManagerPool.Singleton.GetDbConnection(CfgName))
            {
                var cmd = ManagerPool.Singleton.CreateDbCommand(CfgName);
                cmd.Connection = con;
                cmd.CommandText = sql;

                if (param != null)
                {
                    int index = 0;
                    foreach (dynamic p in param)
                    {
                        if (p is ValueType)
                        {
                            object obj = p;
                            var parameter = cmd.CreateParameter();
                            parameter.Value = obj;
                            parameter.ParameterName = "@" + ValueTypeParam.ParamArray[index++];
                            cmd.Parameters.Add(parameter);
                        }
                        else if (p is object)
                        {
                            object obj = p;
                            var properties = obj.GetType().GetProperties();
                            foreach (var py in properties)
                            {
                                var parameter = cmd.CreateParameter();
                                parameter.Value = py.GetValue(obj);
                                parameter.ParameterName = "@" + py.Name;
                                cmd.Parameters.Add(parameter);

                            }
                        }
                    }
                }
                return cmd.ExecuteScalar();
            }
        }

        //public List<T> Query<T, P>(string sql, P param = default(P)) where T:new ()
        //{
        //    using (var con = ManagerPool.Singleton.GetDbConnection(CfgName))
        //    {
        //        var cmd = con.CreateCommand();
        //        //cmd.Connection = con;
        //        cmd.CommandText = sql;

        //        if (param != null)
        //        {
        //            foreach (var p in typeof(P).GetProperties())
        //            {
        //                IDataParameter parameter = ManagerPool.Singleton.CreateDataParameter(CfgName);
        //                parameter.Value = p.GetValue(param);
        //                parameter.ParameterName = "@" + p.Name;
        //                cmd.Parameters.Add(parameter);
        //            }
        //        }
        //        var reader = cmd.ExecuteReader();
        //       return   reader.ToEntityList<T>();
        //    }
        //}

        public List<T> Query<T>(string sql, params dynamic[] param) where T : new()
        {
            using (var con = ManagerPool.Singleton.GetDbConnection(CfgName))
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;

                if (param != null)
                {
                    int index = 0;
                    foreach(dynamic p in param)
                    {
                        if(p is ValueType)
                        {
                            object obj = p;
                            var parameter = cmd.CreateParameter();
                            parameter.Value =obj;
                            parameter.ParameterName = "@" + ValueTypeParam.ParamArray[index++];
                            cmd.Parameters.Add(parameter);  
                        }
                        else if(p is object)
                        {
                            object obj = p;
                            var properties = obj.GetType().GetProperties();
                            foreach (var py in properties)
                            {
                                var parameter = cmd.CreateParameter();
                                parameter.Value = py.GetValue(obj);
                                parameter.ParameterName = "@" + py.Name;
                                cmd.Parameters.Add(parameter);
                               
                            }
                        }
                    }
                }
                var reader = cmd.ExecuteReader();
                return reader.ToEntityList<T>();
            }
        }

        public bool SqlBulkCopy<P>(string sql, List<P> lst = null)
        {
            try
            {
                using (var con = ManagerPool.Singleton.GetDbConnection(CfgName))
                {
                    var tran = con.BeginTransaction();
                    var cmd = ManagerPool.Singleton.CreateDbCommand(CfgName);
                    cmd.Connection = con;
                    cmd.CommandText = sql;
                    cmd.Transaction = tran;
                    //
                    int index = sql.ToLower().IndexOf("values");
                    string sqlvalue = sql.Substring(index + 6);
                    string sqlItem = sqlvalue;
                    var Properties = typeof(P).GetProperties();
                    if (lst != null)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append(sql.Substring(0, index + 7));
                        foreach (var item in lst)
                        {
                            sqlItem = sqlvalue;
                            foreach (var p in Properties)
                            {
                                sqlItem = sqlItem.Replace("@" + p.Name, p.GetValue(item).ToString());
                            }
                            builder.Append(sqlItem + ",");
                        }
                        cmd.CommandText = builder.ToString();
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
