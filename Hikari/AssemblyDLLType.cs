using System;
using System.Collections.Generic;
using System.Text;

/**
* 命名空间: Hikari 
* 类 名： Class1
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace Hikari
{
    /// <summary>
    /// 功能描述    ：AssemblyDLLType   
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/26 23:21:52 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/26 23:21:52 
    /// </summary>
    class AssemblyDLLType
    {
        /// <summary>
        /// 连接类型
        /// </summary>
          public Type ConnectType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Type CommandType { get; set; }

        /// <summary>
        /// 
        /// </summary>
          public Type DataAdapterType { get; set; }
         
        /// <summary>
        /// 
        /// </summary>
          public Type ParameterType { get; set; }

    }
}
