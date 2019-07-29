using System.Collections.Generic;
using System.Data;

namespace HikariAPI
{

    /// <summary>
    /// 扩展字符串以实现参数化直接传参
    /// </summary>
    public static class SQLParameterExtension
    {

        public static string CfgName = null;

        #region

        /// <summary>
        /// DML执行
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int Execute(this string sql, params dynamic[] param)
        {
           return new SqlMapper(CfgName).Execute(sql, param);
        }

        /// <summary>
        /// 获取第一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object ExecuteScalar(this string sql, params dynamic[] param)
        {
            return new SqlMapper(CfgName).ExecuteScalar(sql, param);
        }

        /// <summary>
        /// 查询转换实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<T> Query<T>(string sql, params dynamic[] param) where T : new()
        {
            return new SqlMapper(CfgName).Query<T>(sql, param);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSet QueryData(string sql, params dynamic[] param)
        {
            return new SqlMapper(CfgName).QueryData(sql, param);
        }


        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="sql"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static void BulkCopy<P>(List<P> lst)
        {
            new SqlMapper(CfgName).BulkCopy(lst);
        }


        /// <summary>
        ///计划使用 insert into table(XXX,XXX)values (XXX,XXX),(XXX,XXX)
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="lst"></param>
        public static void SqlBulk<P>(List<P> lst = null)
        {
            new SqlMapper(CfgName).SqlBulk(lst);
        }

      

        #endregion

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int Add<T>(T obj)
        {
           return new SqlMapper(CfgName).Add(obj);
        }

        /// <summary>
        /// 为了支持匿名类；SQL为NULL则在Val查找TableName属性；
        /// </summary>
        /// <param name="sql">参数化SQL语句</param>
        /// <param name="val">数据</param>
        /// <returns></returns>
        public static int Add(dynamic val, string sql = null)
        {
            return new SqlMapper(CfgName).Add(val,sql);
        }

        #region 方便Model属性作为参数，支持匿名类

        /// <summary>
        /// 支持匿名类;使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int ExecuteUpdateModel(string sql, dynamic entity)
        {
            //反射实体
           return new SqlMapper(CfgName).ExecuteUpdateModel(sql, entity);
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static object ExecuteScalarModel(string sql, dynamic entity)
        {
            //反射实体
            return new SqlMapper(CfgName).ExecuteScalarModel(sql, entity);
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static DataSet ExecuteQueryModel(string sql, dynamic entity)
        {
            //反射实体
          return  new SqlMapper(CfgName).ExecuteQueryModel(sql, entity);
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IDataReader ExecuteQueryReaderModel(this string sql, dynamic entity)
        {
            //反射实体
          return  new SqlMapper(CfgName).ExecuteQueryReaderModel(sql, entity);
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<T> ExecuteQueryEntity<T>(this string sql, dynamic entity)
        {
            //反射实体
          return  new SqlMapper(CfgName).ExecuteQueryEntity<T>(sql, entity);
        }

        #endregion

    }
}
