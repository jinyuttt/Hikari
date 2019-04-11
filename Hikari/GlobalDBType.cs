using System.Collections.Generic;
using System.IO;
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
    /// 功能描述    ：GlobalDBType  加载数据库驱动全局信息
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/26 2:54:33 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/26 2:54:33 
    /// </summary>
    public class GlobalDBType
    {
        private static Dictionary<string, DriverDLL> dicDBType = new Dictionary<string, DriverDLL>();

        private static readonly object lock_obj = new object();

        /// <summary>
        /// 加载过的全局文件
        /// </summary>
        private static List<string> lstDBType = new List<string>();

        /// <summary>
        /// 是否已经初始化过
        /// </summary>
        private static volatile bool isInit = false;


        /// <summary>
        /// 初始化已经存在的信息
        /// </summary>
        private static void Init()
        {
           
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
            if (!isInit)
            {
                Init();
                isInit = true;
            }

            //2019-04-06
            if(!File.Exists(dbXml))
            {
                Logger.Singleton.Warn("没有全局配置XML文件," + dbXml);
                return;
            }
            if (!lstDBType.Contains(dbXml))
            {
                //没有加载过的全局文件才加载
                XmlDocument doc = new XmlDocument();
                doc.Load(dbXml);
                foreach (XmlNode child in doc.DocumentElement.ChildNodes)
                {

                    XmlNode dll = child.SelectSingleNode("DriverDLL");
                    // XmlNode cls = child.SelectSingleNode("DriverClass");
                    DriverDLL driver = new DriverDLL();
                    driver.DBType = child.Name;
                    driver.DriverDLLName = dll == null ? "" : dll.OuterXml;
                    dicDBType[driver.DBType] = driver;
                }
                lstDBType.Add(dbXml);
            }
        }

        /// <summary>
        /// 获取dll名称信息
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
