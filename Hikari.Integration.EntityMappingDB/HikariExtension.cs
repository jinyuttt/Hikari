#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：Hikari.Integration.Entity
* 项目描述 ：model与DB转换
* 类 名 称 ：HikariExtension
* 类 描 述 ：model与DB转换
* 命名空间 ：Hikari.Integration.Entity
* CLR 版本 ：4.0.30319.42000
* 作    者 ：jinyu
* 创建时间 ：2019
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ jinyu 2019. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using System.Collections.Generic;
using System.Data;
using EntityMappingDB;

namespace Hikari.Integration.Entity
{
    /* ============================================================================== 
* 功能描述：HikariExtension 
* 创 建 者：jinyu 
* 创建日期：2019 
* 更新时间 ：2019
* ==============================================================================*/
    public static  class HikariExtension
    {

        /// <summary>
        /// 扩展采用Emit对DataTable转换List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToEntity<T>(this DataTable dt)
        {
           return dt.ToEntityList<T>();
        }

        /// <summary>
        /// 扩展采用Emit对DataReader转换List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rd"></param>
        /// <returns></returns>
        public static List<T> ToEntity<T>(this IDataReader rd)
        {
           return rd.ToEntityList<T>();
        }

        /// <summary>
        /// 扩展采用Emit对将List转换DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static DataTable FromEntity<T>(this IList<T> lst)
        {
           return lst.FromEntityToTable<T>();
        }

        /// <summary>
        /// 扩展采用Emit对将List转换DataTable带特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static DataTable FromEntityAttribute<T>(this IList<T> lst)
        {
            return lst.FromEntityToTableMap<T>();
        }
    }
}
