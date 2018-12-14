#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：Hikari.Manager
* 项目描述 ：数据库连接池
* 类 名 称 ：ManagerPool
* 类 描 述 ：数据库连接池管理;允许多数据库配置，按照配置文件名称获取连接
*            例如：MySql_Hikari.cfg
* 命名空间 ：Hikari.Manager
* CLR 版本 ：4.0.30319.42000
* 作    者 ：jinyu
* 创建时间 ：2018
* 更新时间 ：2018
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ jinyu 2018. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion


using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Hikari.Manager
{
    /* ============================================================================== 
    * 功能描述：ManagerPool 线程池管理；按照配置文件名称获取连接
    * 创 建 者：jinyu 
    * 修 改 者：jinyu 
    * 创建日期：2018
    * 修改日期：2018 
    * ==============================================================================*/

    public  class ManagerPool
    {
        /// <summary>
        /// 单例
        /// </summary>
        public readonly static ManagerPool Instance = new ManagerPool();
        private readonly object lock_obj = new object();//全局锁

        /// <summary>
        /// 线程池
        /// </summary>
        private Dictionary<string, HikariDataSource> dicSource = new Dictionary<string, HikariDataSource>();
        private string cfgPath = "DBPoolCfg";//配置目录
        private const string CfgFile = "Hikari";//默认配置文件
        private const string PreCfg= "_Hikari";//配置文件后缀
        private const string CfgExtension = ".cfg";//配置文件后缀

        /// <summary>
        /// 线程连接
        /// </summary>
        private ConcurrentDictionary<int, IDbConnection> dicCons = new ConcurrentDictionary<int, IDbConnection>();

        /// <summary>
        /// 所有连接池配置文件路径
        /// 默认：DBPoolCfg
        /// </summary>
        public string PoolCfgPath { get { return cfgPath; } set { cfgPath = value; } }

        /// <summary>
        /// 数据库驱动DLL配置文件；
        /// 这里必须是唯一文件了
        /// 默认：DBPoolCfg/DBType.xml
        /// </summary>
        public string PoolDriverXML { get; set; }
       

        /// <summary>
        /// 同线程获取连接是否关闭前一个连接
        /// 默认：false
        /// </summary>
        public  bool IsThreadClose { get; set; }

        /// <summary>
        /// 统一设置驱动目录;
        /// 不会覆盖单个配置的设置
        /// 默认：DBDirvers
        /// </summary>
        public string DirverDir { get; set; }


        private ManagerPool()
        {
            //默认
            PoolDriverXML = Path.Combine("DBPoolCfg", "DBType.xml");
            DirverDir = "DBDirvers";
            CheckValiate();
        }

        /// <summary>
        /// 监测已经关闭的
        /// 只是移除无效的，和性能无关
        /// </summary>
        private void CheckValiate()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000000);
                List<IDbConnection> lst = new List<IDbConnection>(100);
                foreach(var item in dicCons)
                {
                    //遍历查找已经关闭的对象
                    HikariConnection hikari = item.Value as HikariConnection;
                    if(hikari!=null)
                    {
                       
                        if(hikari.IsClosed)
                        {
                            //没有加锁，直接移除会导致功能受影响
                            lst.Add(item.Value);
                        }
                    }

                }
                //已经关闭的
                if (lst.Count > 0)
                {
                    //避免刚刚进入的被替换；主要是解决线程不是新线程的问题
                    int[] keys = new int[dicCons.Count];
                    dicCons.Keys.CopyTo(keys, 0);
                    IDbConnection connection = null;
                    foreach(int key in keys)
                    {
                       if( dicCons.TryGetValue(key,out  connection))
                        {
                            if(lst.Contains(connection))
                            {
                                dicCons.TryRemove(key, out connection);
                            }
                        }
                    }
                }
                //
                CheckValiate();//递归返回
            });
        }


        /// <summary>
        /// 加载连接池配置
        /// 创建数据库连接池
        /// </summary>
        /// <param name="name">配置文件名称</param>
        /// <returns></returns>
        private IDbConnection CreatePool(string name)
        {
            //只是在加载配置文件时同步；其它不同步
            lock (lock_obj)
            {
                HikariDataSource hikari = null;
                if (dicSource.TryGetValue(name, out hikari))
                {
                    //先取一次
                    return hikari.GetConnection();
                }
                else
                {
                    //配置文件
                    string file = Path.Combine(cfgPath, name + CfgExtension);
                    if (!File.Exists(file))
                    {
                        throw new Exception("没有配置文件" + file);
                    }
                    HikariConfig hikariConfig = new HikariConfig();
                    hikariConfig.DriverDir = null;//不再使用原来的默认
                    hikariConfig.LoadConfig(file);
                    if(string.IsNullOrEmpty(hikariConfig.DriverDir))
                    {
                       //说明没有在文件中配置
                        hikariConfig.DriverDir = this.DirverDir;
                    }
                    hikariConfig.DBTypeXml = this.PoolDriverXML;
                    hikari = new HikariDataSource(hikariConfig);
                    dicSource[name] = hikari;
                    return hikari.GetConnection();
                }
            }
        }

        /// <summary>
        /// 获取同线程已经获取的线程
        /// 没有在相同线程中使用则不会
        /// </summary>
        /// <returns></returns>
        public bool GetThreadDbConnection(out IDbConnection connection)
        {
            return dicCons.TryGetValue(Thread.CurrentThread.ManagedThreadId, out connection);
        }


        /// <summary>
        /// 获取连接
        /// 例如：配置文件MySql_Hikari.cfg，name=MySql
        /// 
        /// </summary>
        /// <param name="name">配置文件名称，区分大小写</param>
        /// <returns></returns>
        public IDbConnection GetDbConnection(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = CfgFile;//使用默认名称
            }
            else
            {
                name = name + PreCfg;//组合名称
            }
            HikariDataSource hikari = null;
            int threadid = Thread.CurrentThread.ManagedThreadId;
            if (dicSource.TryGetValue(name, out hikari))
            {
                IDbConnection con = hikari.GetConnection();
                if (IsThreadClose)
                {
                    IDbConnection connection = null;
                    if (dicCons.TryRemove(threadid, out connection))
                    {
                        connection.Close();
                    }
                }
                dicCons[threadid] = con;
                return con;
            }
            else
            {
                IDbConnection con = CreatePool(name);
                dicCons[threadid] = con;
                return con;
            }
        }

        #region ADO.NET对象

        /// <summary>
        /// 获取驱动对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDbDataAdapter CreateDataAdapter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = CfgFile;//使用默认名称
            }
            HikariDataSource hikari = null;
            if (dicSource.TryGetValue(name, out hikari))
            {
                return hikari.DataAdapter;
            }
            else
            {
                CreatePool(name);
                if (dicSource.TryGetValue(name, out hikari))
                {
                    return hikari.DataAdapter;
                }
                else
                { return null; }

            }
        }

        /// <summary>
        /// 获取驱动对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDbCommand CreateDbCommand(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = CfgFile;//使用默认名称
            }
            else
            {
                name = name + "_" + CfgFile;
            }
            name = name.Trim();
            HikariDataSource hikari = null;
            if (dicSource.TryGetValue(name, out hikari))
            {
                return hikari.DbCommand;
            }
            else
            {
                CreatePool(name);
                if (dicSource.TryGetValue(name, out hikari))
                {
                    return hikari.DbCommand;
                }
                else
                { return null; }

            }
        }

        /// <summary>
        /// 获取驱动对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDbDataParameter CreateDataParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = CfgFile;//使用默认名称
            }
            HikariDataSource hikari = null;
            if (dicSource.TryGetValue(name, out hikari))
            {
                return hikari.DataParameter;
            }
            else
            {
                CreatePool(name);
                if (dicSource.TryGetValue(name, out hikari))
                {
                    return hikari.DataParameter;
                }
                else
                { return null; }

            }
        }
        #endregion 

        /// <summary>
        /// 清理连接池
        /// </summary>
        /// <param name="name"></param>
        public void ClearPool(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = CfgFile;//使用默认名称
            }
            name = name.Trim();
            HikariDataSource hikari = null;
            lock (lock_obj)
            {
                if (dicSource.TryGetValue(name, out hikari))
                {
                    dicSource.Remove(name);
                    hikari.Close();
                }

            }
        }

        /// <summary>
        /// 关闭所有
        /// </summary>
        public void ClearAllPool()
        {
            lock (lock_obj)
            {
                string[] keys = new string[dicSource.Count];
                dicSource.Keys.CopyTo(keys, 0);
                foreach (string name in keys)
                {
                    ClearPool(name);
                }
            }
        }
    }
}
