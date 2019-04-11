using System.Collections.Generic;
using System.Data;

namespace Hikari.Manager
{

    /// <summary>
    /// SQL操作扩展
    /// </summary>
    public static  class ManagerExtension
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
        public static DataSet ExecuteQuery(this ManagerPool manager,string querySql, string name=null,Dictionary<string, object> valuePairs = null)
        {
            if (string.IsNullOrEmpty(querySql))
            {
                return null;
            }
            var source= manager.GetHikariDataSource(name);
            return  source.ExecuteQuery(querySql, valuePairs);
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
        public static IDataReader ExecuteQueryReader(this ManagerPool manager, string querySql,string name=null, Dictionary<string, object> valuePairs = null)
        {
            if(string.IsNullOrEmpty(querySql))
            {
                return null;
            }
            var source = manager.GetHikariDataSource(name);
            return source.ExecuteQueryReader(querySql, valuePairs);

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
        public static int ExecuteUpdate(this ManagerPool manager, string Sql,string name, Dictionary<string, object> valuePairs = null)
        {
            if(string.IsNullOrEmpty(Sql))
            {
                return -1;
            }
            var source = manager.GetHikariDataSource(name);
            return  source.ExecuteUpdate(Sql, valuePairs);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Sql"></param>
        /// <param name="valuePairs"></param>
        /// <returns></returns>
        public static object ExecuteScalar(this ManagerPool manager, string Sql,string name=null, Dictionary<string, object> valuePairs = null)
        {
            if(string.IsNullOrEmpty(Sql))
            {
                return null;
            }
            var source = manager.GetHikariDataSource(name);
            return source.ExecuteScalar(Sql, valuePairs);
        }

        /// <summary>
        /// 批量导入
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="name"></param>
        /// <param name="dt"></param>
        public static void BluckCopy(this ManagerPool manager,string name=null, DataTable dt=null)
        {
            if(dt==null||dt.Rows.Count==0)
            {
                return;
            }
            var source = manager.GetHikariDataSource(name);
            source.BulkCopy(dt);
        }
    }
}
