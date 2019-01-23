#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：Hikari.Integration.Models
* 项目描述 ：
* 类 名 称 ：MapColumn
* 类 描 述 ：
* 命名空间 ：Hikari.Integration.Models
* CLR 版本 ：4.0.30319.42000
* 作    者 ：jinyu
* 创建时间 ：2019
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ jinyu 2019. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion



using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hikari.Integration.Models
{
    /* ============================================================================== 
* 功能描述：MapColumn  映射列
* 创 建 者：jinyu 
* 创建日期：2019 
* 更新时间 ：2019
* ==============================================================================*/

   public class MapColumn
    {
        public PropertyInfo Property { get; set; }

       public string ColumnName { get; set; }

    }
}
