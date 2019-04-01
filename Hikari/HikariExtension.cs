using System.Collections.Generic;
using System.Data;

namespace Hikari
{

    /// <summary>
    /// 扩展SQL操作
    /// </summary>
    public static  class HikariExtension
    {

        /// <summary>
        /// 查询数据
        /// 例如：select * from Person where id=@ID
        /// valuePairs["ID"]=1;
        /// </summary>
        /// <param name="source"></param>
        /// <param name="querySql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(this HikariDataSource source, string querySql, Dictionary<string,object> valuePairs=null)
        {
            using (var con = source.GetConnection())
            {
                var cmd = con.CreateCommand();
                cmd.CommandText = querySql;
                //cmd.Connection = con;
                if (valuePairs != null)
                {
                    foreach(var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName ="@"+kv.Key;
                        p.Value = kv.Value;
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
            cmd.Connection = con;
            if (valuePairs != null)
            {
                foreach (var kv in valuePairs)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@" + kv.Key;
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
                cmd.Connection = con;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@" + kv.Key;
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
                cmd.Connection = con;
                if (valuePairs != null)
                {
                    foreach (var kv in valuePairs)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@" + kv.Key;
                        p.Value = kv.Value;
                        cmd.Parameters.Add(p);
                    }
                }
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// 批量导入
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dt"></param>
        public static void BulkCopy(this HikariDataSource source,DataTable dt)
        {
            var bluk=  source.GetBulkCopy();
            bluk.BulkCopy(dt);
        }
    }
}
