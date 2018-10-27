using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

/**
* 命名空间: Hikari 
* 类 名： HikariConnect
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace Hikari
{
    /// <summary>
    /// 功能描述    ：HikariConnect  
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/25 1:55:36 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/25 1:55:36 
    /// </summary>
   public class HikariConnection: ProxyConnection
    {
        public HikariConnection(PoolEntry poolEntry, IDbConnection connection, long now):base(poolEntry,connection,now)
        {

        }
    }
}
