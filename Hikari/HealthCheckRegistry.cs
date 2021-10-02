#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：Hikari
* 项目描述 ：
* 类 名 称 ：healthCheckRegistry
* 类 描 述 ：
* 命名空间 ：Hikari
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hikari
{
    /* ============================================================================== 
* 功能描述：healthCheckRegistry  监测数据库连接异常
* 创 建 者：jinyu 
* 创建日期：2019 
* 更新时间 ：2019
* ==============================================================================*/

    public class HealthCheckRegistry
    {
        public static readonly HealthCheckRegistry Singleton = new HealthCheckRegistry();
        private ConcurrentDictionary<string, HealthItem> dicPool = null;
        private readonly int waitTime = 1000;
        private const int ConnectTime = 250;//毫秒
        private Thread checkThread = null;


        private HealthCheckRegistry()
        {
            dicPool = new ConcurrentDictionary<string, HealthItem>();
            checkThread = new Thread(Start);
            checkThread.Name = "HiKariHealth";
            checkThread.IsBackground = true;
            checkThread.Start();
        }

        /// <summary>
        /// 添加异常
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pool"></param>
        public void Add(string name, PoolBase pool)
        {
            dicPool[name] = new HealthItem() { Pool = (HikariPool)pool, Count = 0 };
        }

        /// <summary>
        /// 开始监测
        /// </summary>
        private void Start()
        {
            while (true)
            {

                Task.Factory.StartNew(() =>
                {
                    List<string> lst = new List<string>();
                    foreach (var kv in dicPool)
                    {

                        var con = kv.Value.Pool.GetConnection(ConnectTime);
                        if (con == null)
                        {
                            kv.Value.Count++;
                            kv.Value.Tick = DateTime.Now.Ticks;
                            if (kv.Value.Count > 10)
                            {
                                Logger.Singleton.WarnFormat("连接池异常，连接池名称：{0},连接池配置：{1}", kv.Key, kv.Value.Pool.ConnectStr);
                            }
                        }
                        else
                        {
                            kv.Value.Count--;
                            con.Dispose();
                            if (kv.Value.IsSucess)
                            {
                                lst.Add(kv.Key);
                            }
                        }
                    }
                    if (lst.Count > 0)
                    {
                        HealthItem item = null;
                        foreach (string k in lst)
                        {

                            dicPool.TryRemove(k, out item);
                        }
                    }
                });

                Thread.Sleep(waitTime);
            }
        }
    }

    /// <summary>
    /// 连接池存储项
    /// </summary>
    public class HealthItem
    {
        private const int TickS = 10000000;
        internal HikariPool Pool { get; set; }
        public int Count { get; set; }
        public long Tick { get; set; }

        /// <summary>
        /// 5秒内没有延迟
        /// </summary>
        public bool IsSucess
        {
            get
            {
                if (Count < 0 && (DateTime.Now.Ticks - Tick) / TickS > 5)
                {
                    return true;
                }
                return false;
            }
        }

    }
}
