using System.Collections.Generic;
using System.Xml;

/**
* 命名空间: Hikari 
* 类 名： GlobalDBType
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace Hikari
{
    /// <summary>
    /// 功能描述    ：GlobalDBType  加载数据库信息配置
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/26 2:54:33 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/26 2:54:33 
    /// </summary>
    public class GlobalDBType
    {
        private static Dictionary<string, DriverDLL> dicDBType = new Dictionary<string, DriverDLL>();

        private static object lock_obj = new object();

        /// <summary>
        /// 已
        /// </summary>
        private static void Init()
        {
            //oralce
            lock (lock_obj)
            {
                //Oracle
                DriverDLL driverDLL = new DriverDLL();
                driverDLL.DriverDLLName = "Oracle.ManagedDataAccess";
                driverDLL.DBType = "Oracle";
                dicDBType[driverDLL.DBType] = driverDLL;
                //MySql
                driverDLL = new DriverDLL();
                driverDLL.DriverDLLName = "MySql.Data";

                driverDLL.DBType = "MySql";
                dicDBType[driverDLL.DBType] = driverDLL;
                //SQLServer
                driverDLL = new DriverDLL();
                driverDLL.DriverDLLName = "System.Data";
                driverDLL.DBType = "SqlServer";
                dicDBType[driverDLL.DBType] = driverDLL;
                //PostgreSQL
                driverDLL = new DriverDLL();
                driverDLL.DriverDLLName = "Npgsql";

                driverDLL.DBType = "PostgreSQL";
                dicDBType[driverDLL.DBType] = driverDLL;
            }
        }
        /// <summary>
        /// 加载数据库信息
        /// 读取数据库类型的DLL文件
        /// </summary>
        /// <param name="dbXml"></param>
        public static  void LoadXml(string dbXml)
        {
            //
            Init();
             XmlDocument doc = new XmlDocument();
            doc.Load(dbXml);
            foreach(XmlNode child in doc.DocumentElement.ChildNodes)
            {
               
               XmlNode dll= child.SelectSingleNode("DriverDLL");
                XmlNode cls = child.SelectSingleNode("DriverClass");
                DriverDLL driver = new DriverDLL();
                driver.DBType = child.Name;
                driver.DriverDLLName = dll == null ? "" : dll.OuterXml;
              
                dicDBType[driver.DBType] = driver;
            }
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="dbtype"></param>
        /// <returns></returns>
        public static DriverDLL GetDriver(string dbtype)
        {
            DriverDLL driver = null;
            dicDBType.TryGetValue(dbtype, out driver);
            return driver;
        }
    }
}
