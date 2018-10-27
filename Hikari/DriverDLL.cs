using System;
using System.Collections.Generic;
using System.Text;

/**
* 命名空间: Hikari 
* 类 名： DriverDLL
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace Hikari
{
    /// <summary>
    /// 功能描述    ：DriverDLL  
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/26 2:52:27 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/26 2:52:27 
    /// </summary>
   public class DriverDLL
    {
        /// <summary>
        /// dll名称
        /// </summary>
        public string DriverDLLName { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DBType { get; set; }
    }
}
