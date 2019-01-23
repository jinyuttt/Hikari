#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：ConsoleApp2
* 项目描述 ：
* 类 名 称 ：Person
* 类 描 述 ：
* 命名空间 ：ConsoleApp2
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hikari.Integration.Models;
namespace ConsoleApp2
{
    /* ============================================================================== 
* 功能描述：Person 
* 创 建 者：jinyu 
* 创建日期：2019 
* 更新时间 ：2019
* ==============================================================================*/

  public  class Person
    {
        public int ID { get; set; }

        [DataField("Name")]
        public string PName { get; set; }

        public int Age { get; set; }

        public DateTime Create { get; set; }


    }
}
