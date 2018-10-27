using System;
using System.Data;

namespace Hikari
{
    /// <summary>
    /// 创建代理对象
    /// </summary>
    internal class ProxyFactory
    {
        /// <summary>
        /// 创建代理对象
        /// </summary>
        /// <param name="poolEntry"></param>
        /// <param name="connection"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        internal static IDbConnection GetProxyConnection(PoolEntry poolEntry, IDbConnection connection, long now)
        {
            try
            {
                return new HikariConnection(poolEntry, connection, now);
            }
            catch (Exception ex)
            {
                throw new Exception("获取失败78:" + ex.Message);
            }
        }
    }
}