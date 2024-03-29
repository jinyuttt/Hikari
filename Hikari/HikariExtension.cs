﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Hikari
{

    /// <summary>
    /// 扩展SQL操作
    /// </summary>
    public static class HikariExtension
    {

        /// <summary>
        /// 类型映射
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static DbType GetDbType(string name)
        {

            DbType db = DbType.Object;
            switch (name)
            {
                case "String":
                case "Char[]":
                    db = DbType.String;
                    break;
                    ;
                case "Number":
                    db = DbType.Int32;
                    break;
                case "Byte[]":
                    db = DbType.Binary;
                    break;
                case "Boolen":
                    db = DbType.Boolean;
                    break;
                case "Byte":
                    db = DbType.Byte;
                    break;
                case "DateTime":
                    db = DbType.DateTime;
                    break;
                case "Decimal":
                    db = DbType.Decimal;
                    break;
                case "Double":
                    db = DbType.Double;
                    break;
                case "Guid":
                    db = DbType.Guid;
                    break;
                case "Int16":
                    db = DbType.Int16;
                    break;
                case "Int32":
                    db = DbType.Int32;
                    break;

                case "Int64":
                    db = DbType.Int64;
                    break;
                case "UInt16":
                    db = DbType.UInt16;
                    break;
                case "UInt32":
                    db = DbType.UInt32;
                    break;
                case "UInt64":
                    db = DbType.UInt64;
                    break;
                case "Xml":
                    db = DbType.Xml;
                    break;
                case "VarNumeric":
                    db = DbType.VarNumeric;
                    break;
                case "BitArray":
                    db = DbType.Boolean;
                    break;
            }
            return db;


        }

        /// <summary>
        /// 对字符串值转换处理
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        private static object CheckString(string dataType, object v)
        {
            if(dataType == null) return v;
            if (v.GetType().Name == "String" && dataType != "String")
            {
                if (Enum.TryParse(typeof(TypeCode), dataType, out var result))
                {
                    v = Convert.ChangeType(v, (TypeCode)result);
                }
            }
            return v;
        }

        /// <summary>
        /// 查询数据
        /// 例如：select * from Person where id=@ID
        /// valuePairs["ID"]=1;
        /// </summary>
        /// <param name="source"></param>
        /// <param name="querySql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(this HikariDataSource source, string querySql, Dictionary<string, object> valuePairs = null)
        {
            using (var con = source.GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = querySql;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@" + kv.Key.TrimStart('@');
                        p.Value =kv.Value;
                        cmd.Parameters.Add(p);
                    }
                }
                var da = source.DataAdapter;
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Dispose();
                return ds;
            }
        }

        /// <summary>
        /// 查询数据
        /// 例如：select * from Person where id=@ID
        ///  valuePairs["ID"]=1;
        /// </summary>
        /// <param name="source"></param>
        /// <param name="querySql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static IDataReader ExecuteQueryReader(this HikariDataSource source, string querySql, Dictionary<string, object> valuePairs = null)
        {
            var con = source.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = querySql;

            if (valuePairs != null)
            {
                foreach (var kv in valuePairs)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@" + kv.Key.TrimStart('@');
                    p.Value = kv.Value;
                    cmd.Parameters.Add(p);
                }
            }
            return cmd.ExecuteReader();
        }

        /// <summary>
        /// 执行语句
        /// 例如：insert into Person(id,name) values(@ID,@Name)
        /// valuePairs["ID"]=1;valuePairs["Name"]="jason";
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Sql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static int ExecuteUpdate(this HikariDataSource source, string Sql, Dictionary<string, object> valuePairs = null)
        {
            using (var con = source.GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = Sql;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@" + kv.Key.TrimStart('@');
                        p.Value = kv.Value;
                        cmd.Parameters.Add(p);
                    }
                }
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Sql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static object ExecuteScalar(this HikariDataSource source, string Sql, Dictionary<string, object> valuePairs = null)
        {
            using (var con = source.GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = Sql;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@" + kv.Key.TrimStart('@');
                        p.Value = kv.Value;
                        cmd.Parameters.Add(p);
                    }
                }
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Sql">存储过程名称</param>
        /// <param name="valuePairs"></param>
        /// <param name="outvalues">输出参数</param>
        /// <returns></returns>
        public static int ExecuteStoredProcedure(this HikariDataSource source, string Sql, Dictionary<string, object> valuePairs = null, Dictionary<string, object> outvalues = null)
        {
            using (var con = source.GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = Sql;
                cmd.CommandType = CommandType.StoredProcedure;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = kv.Key;
                        p.Value = kv.Value;
                        cmd.Parameters.Add(p);
                    }
                }
                if (outvalues != null)
                {
                    foreach (var kv in outvalues)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = kv.Key;
                        p.Value = kv.Value;
                        p.Direction = ParameterDirection.Output;

                        cmd.Parameters.Add(p);
                    }
                }
                int r = cmd.ExecuteNonQuery();

                //取出输出
                if (outvalues != null)
                {
                    foreach (var kv in outvalues.Keys)
                    {
                        IDbDataParameter p = (IDbDataParameter)cmd.Parameters[kv];
                        outvalues[kv] = p.Value;
                    }
                }
                return r;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Sql">存储过程名称</param>
        /// <param name="valuePairs"></param>
        /// <param name="outvalues">输出参数</param>
        /// <returns></returns>
        public static DataSet ExecuteStoredProcedureWithValue(this HikariDataSource source, string Sql, Dictionary<string, object> valuePairs = null, Dictionary<string, object> outvalues = null)
        {
            using (var con = source.GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = Sql;
                cmd.CommandType = CommandType.StoredProcedure;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@" + kv.Key.TrimStart('@');
                        p.Value = kv.Value;
                        cmd.Parameters.Add(p);
                    }
                }
                if (outvalues != null)
                {
                    foreach (var kv in outvalues)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@" + kv.Key.TrimStart('@');
                        p.Value = kv.Value;
                        p.Direction = ParameterDirection.Output;

                        cmd.Parameters.Add(p);
                    }
                }
                var adapter = source.DataAdapter;
                adapter.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                //取出输出
                if (outvalues != null)
                {
                    foreach (var kv in outvalues.Keys)
                    {
                        IDbDataParameter p = (IDbDataParameter)cmd.Parameters[kv];
                        outvalues[kv] = p.Value;
                    }
                }
                return ds;
            }
        }

        /// <summary>
        /// 批量导入
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dt"></param>
        public static void BulkCopy(this HikariDataSource source, DataTable dt)
        {
            var bluk = source.GetBulkCopy();
            bluk.BulkCopy(dt);
        }


        /// <summary>
        /// 查询数据
        /// 例如：select * from Person where id=@ID
        /// valuePairs["ID"]=1;
        /// </summary>
        /// <param name="source"></param>
        /// <param name="querySql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(this HikariDataSource source, string querySql, Dictionary<string, SqlValue> valuePairs = null)
        {
            using (var con = source.GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = querySql;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        HikariExtensionHelpers.SetParameter(p, kv.Value.Type, kv.Value.Value);
                        
                        p.ParameterName = "@" + kv.Key.TrimStart('@');
                        p.Value =CheckString(kv.Value.Type, kv.Value.Value);
                        p.DbType = GetDbType(kv.Value.Type);
                        cmd.Parameters.Add(p);
                    }
                }
                var da = source.DataAdapter;
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Dispose();
                return ds;
            }
        }

        /// <summary>
        /// 转换参数
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="valuePairs">SQL参数</param>
        /// <returns></returns>
        private static Dictionary<string, SqlValue> ConvertSqlValue<T>(Dictionary<string, T> valuePairs) where T:class
        {
            Dictionary<string, SqlValue> keyValuePairs = new Dictionary<string, SqlValue>();
            if (valuePairs == null || valuePairs.Count == 0)
            {
                return keyValuePairs;
            }
            foreach (var kv in valuePairs)
            {
                var sqlvalue = HikariExtensionHelpers.ConvertSqlValue(kv.Value);
                keyValuePairs.Add(kv.Key, sqlvalue);
            }
            return keyValuePairs;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T">泛型参数，需要SqlValue相同属性</typeparam>
        /// <param name="source"></param>
        /// <param name="querySql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static DataSet ExecuteQuery<T>(this HikariDataSource source, string querySql, Dictionary<string, T> valuePairs = null) where T:class
        {
            var dic = ConvertSqlValue<T>(valuePairs);
            return ExecuteQuery(source, querySql, dic);
        }

        /// <summary>
        /// 查询数据
        /// 例如：select * from Person where id=@ID
        ///  valuePairs["ID"]=1;
        /// </summary>
        /// <param name="source"></param>
        /// <param name="querySql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static IDataReader ExecuteQueryReader(this HikariDataSource source, string querySql, Dictionary<string, SqlValue> valuePairs = null)
        {
            var con = source.GetConnection();
            var cmd = con.CreateCommand();
            cmd.CommandText = querySql;

            if (valuePairs != null)
            {
                foreach (var kv in valuePairs)
                {
                    var p = cmd.CreateParameter();
                    HikariExtensionHelpers.SetParameter(p, kv.Value.Type, kv.Value.Value);
                    // HikariExtensionHelpers.SetParameter(kv.Value.Type, p);
                    p.ParameterName = "@" + kv.Key.TrimStart('@');
                    p.Value = CheckString(kv.Value.Type, kv.Value.Value);
                    p.DbType = GetDbType(kv.Value.Type);
                    cmd.Parameters.Add(p);
                }
            }
            return cmd.ExecuteReader();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">需要SqlValue相同属性</typeparam>
        /// <param name="source"></param>
        /// <param name="querySql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static IDataReader ExecuteQueryReader<T>(this HikariDataSource source, string querySql, Dictionary<string, T> valuePairs = null) where T : class
        {
            var dic = ConvertSqlValue<T>(valuePairs);
            return ExecuteQueryReader(source, querySql, dic);
        }

        /// <summary>
        /// 执行语句
        /// 例如：insert into Person(id,name) values(@ID,@Name)
        /// valuePairs["ID"]=1;valuePairs["Name"]="jason";
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Sql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static int ExecuteUpdate(this HikariDataSource source, string Sql, Dictionary<string, SqlValue> valuePairs = null) 
        {
            using (var con = source.GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = Sql;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        p.DbType = GetDbType(kv.Value.Type);
                        p.Value = CheckString(kv.Value.Type, kv.Value.Value);
                        HikariExtensionHelpers.SetParameter(p, kv.Value.Type, kv.Value.Value);
                        
                        p.ParameterName = "@" + kv.Key.TrimStart('@');

                        cmd.Parameters.Add(p);
                    }
                }
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T">需要SqlValue相同属性</typeparam>
        /// <param name="source"></param>
        /// <param name="Sql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static int ExecuteUpdate<T>(this HikariDataSource source, string Sql, Dictionary<string, T> valuePairs = null)where T : class
        {
            var dic = ConvertSqlValue<T>(valuePairs);
            return ExecuteUpdate(source, Sql, dic);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Sql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static object ExecuteScalar(this HikariDataSource source, string Sql, Dictionary<string, SqlValue> valuePairs = null)
        {
            using (var con = source.GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = Sql;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        HikariExtensionHelpers.SetParameter(p, kv.Value.Type, kv.Value.Value);
                        //  HikariExtensionHelpers.SetParameter(kv.Value.Type, p);
                        p.ParameterName = "@" + kv.Key.TrimStart('@');
                        p.Value = CheckString(kv.Value.Type, kv.Value.Value);
                        p.DbType = GetDbType(kv.Value.Type);
                        cmd.Parameters.Add(p);
                    }
                }
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Sql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static object ExecuteScalar<T>(this HikariDataSource source, string Sql, Dictionary<string, T> valuePairs = null)where T : class
        {
            var dic = ConvertSqlValue<T>(valuePairs);
            return ExecuteScalar(source, Sql, dic);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Sql">存储过程名称</param>
        /// <param name="valuePairs"></param>
        /// <param name="outvalues">输出参数</param>
        /// <returns></returns>
        public static int ExecuteStoredProcedure(this HikariDataSource source, string Sql, Dictionary<string, SqlValue> valuePairs = null, Dictionary<string, SqlValue> outvalues = null)
        {
            using (var con = source.GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = Sql;
                cmd.CommandType = CommandType.StoredProcedure;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        HikariExtensionHelpers.SetParameter(p, kv.Value.Type, kv.Value.Value);
                        // HikariExtensionHelpers.SetParameter(kv.Value.Type, p);
                        p.ParameterName = kv.Key;
                        p.Value = CheckString(kv.Value.Type, kv.Value.Value);
                        p.DbType = GetDbType(kv.Value.Type);
                        cmd.Parameters.Add(p);
                    }
                }
                if (outvalues != null)
                {
                    foreach (var kv in outvalues)
                    {
                        var p = cmd.CreateParameter();
                        HikariExtensionHelpers.SetParameter(p, kv.Value.Type, kv.Value.Value);
                        // HikariExtensionHelpers.SetParameter(kv.Value.Type, p);
                        p.ParameterName = kv.Key.TrimStart('@');

                        p.DbType = GetDbType(kv.Value.Type);
                        p.Direction = ParameterDirection.Output;

                        cmd.Parameters.Add(p);
                    }
                }
                int r = cmd.ExecuteNonQuery();

                //取出输出
                if (outvalues != null)
                {
                    foreach (var kv in outvalues.Keys)
                    {
                        IDbDataParameter p = (IDbDataParameter)cmd.Parameters[kv];
                        outvalues[kv].Value = p.Value;
                    }
                }
                return r;
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Sql"></param>
        /// <param name="valuePairs"></param>
        /// <param name="outvalues">输出参数</param>
        /// <returns></returns>
        public static DataSet ExecuteStoredProcedureWithValue(this HikariDataSource source, string Sql, Dictionary<string, SqlValue> valuePairs = null, Dictionary<string, SqlValue> outvalues = null)
        {
            using (var con = source.GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = Sql;
                cmd.CommandType = CommandType.StoredProcedure;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        HikariExtensionHelpers.SetParameter(p, kv.Value.Type, kv.Value.Value);
                        // HikariExtensionHelpers.SetParameter(kv.Value.Type, p);
                        p.ParameterName = kv.Key.TrimStart('@');
                        p.Value = CheckString(kv.Value.Type, kv.Value.Value);
                        p.DbType = GetDbType(kv.Value.Type);
                        cmd.Parameters.Add(p);
                    }
                }
                if (outvalues != null)
                {
                    foreach (var kv in outvalues)
                    {
                        var p = cmd.CreateParameter();
                        HikariExtensionHelpers.SetParameter(p, kv.Value.Type, kv.Value.Value);
                        // HikariExtensionHelpers.SetParameter(kv.Value.Type, p);
                        p.ParameterName = kv.Key;

                        p.DbType = GetDbType(kv.Value.Type);
                        p.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(p);
                    }
                }
                var adapter = source.DataAdapter;
                adapter.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                //取出输出
                if (outvalues != null)
                {
                    foreach (var kv in outvalues.Keys)
                    {
                        IDbDataParameter p = (IDbDataParameter)cmd.Parameters[kv];
                        outvalues[kv].Value = p.Value;
                    }
                }
                return ds;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">需要SqlValue相同属性</typeparam>
        /// <param name="source"></param>
        /// <param name="Sql"></param>
        /// <param name="valuePairs"></param>
        /// <param name="outvalues"></param>
        /// <returns></returns>
        public static DataSet ExecuteStoredProcedureWithValue<T>(this HikariDataSource source, string Sql, Dictionary<string, T> valuePairs = null, Dictionary<string, SqlValue> outvalues = null) where T : class
        {
            var div = ConvertSqlValue<T>(valuePairs);
            return ExecuteStoredProcedureWithValue(source, Sql, div);
        }
    }
}
