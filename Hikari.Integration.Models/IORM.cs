using System;
using System.Collections.Generic;
using System.Text;

namespace Hikari.Integration.Models
{
  public  interface IORM
    {
   
        /// <summary>
        /// 数据查询
        /// </summary>
        /// <typeparam name="T">转换实体</typeparam>
        /// <param name="sql">Sql</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        List<T> Query<T>(string sql,params dynamic[] param) where T : new();

      /// <summary>
      /// 执行SQL
      /// </summary>
      /// <typeparam name="P"></typeparam>
      /// <param name="sql"></param>
      /// <param name="param"></param>
      /// <returns></returns>
        int Execute<P>(string sql, params dynamic[] param);

        /// <summary>
        /// 批量执行
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="sql"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        bool SqlBulkCopy<P>(string sql, List<P> lst = null);


        /// <summary>
        /// 查询单值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        object ExecuteScalar<T>(string sql, params dynamic[] param);
    }
}
