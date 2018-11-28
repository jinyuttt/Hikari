using log4net.Core;
using System;
using System.Collections.Concurrent;
using System.Threading;
/**
* 命名空间: Hikari 
* 类 名： KeepingExecutorService
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace Hikari
{
    /// <summary>
    /// 功能描述    ：KeepingExecutorService  监视连接池对象活动
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/24 22:00:41 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/24 22:00:41 
    /// </summary>
    public class KeepingExecutorService
    {
       
        private ConcurrentQueue<PoolEntry> idleTimeQueue;
        private ConcurrentQueue<PoolEntry> maxLiveQueue;
        private ConcurrentQueue<PoolEntry> userQueue;

        /// <summary>
        /// tick与毫秒的转化值
        /// </summary>
        private static int tickms = 10000;

        /// <summary>
        /// 空闲时间
        /// </summary>
        private int idleTimeOut = 0;

       /// <summary>
       /// 最大生命周期
       /// </summary>
        private int maxLeftTime = 0;

        /// <summary>
        /// 离开时间
        /// </summary>
        private int leakDetectionThreshold = 0;

        public bool IsStop { get; set; }

        public string PoolName { get; set; }

        public KeepingExecutorService(long idleTime,long maxLeft,long usetime)
        {
            this.idleTimeOut =(int) idleTime;
            this.maxLeftTime =(int) maxLeft;
            this.leakDetectionThreshold =(int) usetime;
         
            this.idleTimeQueue = new ConcurrentQueue<PoolEntry>();
            this.maxLiveQueue = new ConcurrentQueue<PoolEntry>();
            this.userQueue = new ConcurrentQueue<PoolEntry>();
            Start();
        }

        /// <summary>
        /// 监测空闲的连接
        /// </summary>
        /// <param name="poolEntry"></param>
        public void ScheduleIdleTimeout(PoolEntry poolEntry)
        {
            idleTimeQueue.Enqueue(poolEntry);
        }

        /// <summary>
        /// 监视连接最大存活
        /// </summary>
        /// <param name="poolEntry"></param>
        public void ScheduleMaxLive(PoolEntry poolEntry)
        {
            maxLiveQueue.Enqueue(poolEntry);
        }

        /// <summary>
        /// 监视连接离开的池的对象
        /// </summary>
        /// <param name="poolEntry"></param>
        public void ScheduleUse(PoolEntry poolEntry)
        {
            userQueue.Enqueue(poolEntry);
        }

        /// <summary>
        /// 开启监视
        /// </summary>
        private void Start()
        {
            Thread idle = new Thread(() =>
            {
                PoolEntry poolEntry = null;
                while (!IsStop)
                {
                    Thread.Sleep(idleTimeOut);
                    int num = idleTimeQueue.Count;
                    long now = DateTime.Now.Ticks;
                    while (num > 0)
                    {
                        if (idleTimeQueue.TryDequeue(out poolEntry))
                        {
                            //超过空闲时间就不需要，标记移除
                            if ((now - poolEntry.AccessedTime) / tickms > idleTimeOut)
                            {
                                poolEntry.CompareAndSetState(IConcurrentBagEntry.STATE_NOT_IN_USE, IConcurrentBagEntry.STATE_REMOVED);
                            }
                            num--;
                            if (poolEntry.State != IConcurrentBagEntry.STATE_REMOVED)
                            {
                                //已经标记移除的不再监测
                                idleTimeQueue.Enqueue(poolEntry);
                            }
                        }
                    }
                }
            });
            idle.Name = PoolName + "_idle";
            idle.IsBackground = true;
            idle.Start();
            //
            Thread maxLeft = new Thread(() =>
            {
                PoolEntry poolEntry = null;
                while (!IsStop)
                {
                    Thread.Sleep(maxLeftTime);
                    int num = maxLiveQueue.Count;
                    long now = DateTime.Now.Ticks;
                    while (num > 0)
                    {
                        if (maxLiveQueue.TryDequeue(out poolEntry))
                        {
                            if ((now - poolEntry.CreateTime) / tickms > maxLeftTime)
                            {
                                poolEntry.CompareAndSetState(IConcurrentBagEntry.STATE_NOT_IN_USE, IConcurrentBagEntry.STATE_REMOVED);
                            }
                            num--;
                            if (poolEntry.State != IConcurrentBagEntry.STATE_REMOVED)
                            {
                                //已经标记移除的不再监测
                                maxLiveQueue.Enqueue(poolEntry);
                            }
                           
                        }
                    }
                }
            });
            maxLeft.Name = PoolName + "_left";
            maxLeft.IsBackground = true;
            maxLeft.Start();
            //
            Thread leakDetection = new Thread(() =>
            {
                PoolEntry poolEntry = null;
                int cout = 10;//延迟10s没有设置就退出；
                while (!IsStop)
                {
                    Thread.Sleep(leakDetectionThreshold);
                    if(leakDetectionThreshold==0)
                    {
                        Thread.Sleep(1000);//延迟1s;
                        cout--;
                        if(cout==0)
                        {
                            break;
                        }
                        continue;
                    }
                    int num = userQueue.Count;
                    long now = DateTime.Now.Ticks;
                    while (num > 0)
                    {
                        if (userQueue.TryDequeue(out poolEntry))
                        {
                            if (poolEntry.State == IConcurrentBagEntry.STATE_IN_USE)
                            {
                                if ((now - poolEntry.AccessedTime) / tickms > leakDetectionThreshold)
                                {
                                    Logger.Singleton.Warn(string.Format("{0}-可能泄露,实体:{1}",PoolName,poolEntry.ID));
                                }
                            }
                            num--;
                            if (poolEntry.State == IConcurrentBagEntry.STATE_IN_USE)
                            {
                                //没有使用的不再监测
                                userQueue.Enqueue(poolEntry);
                            }
                        }
                    }
                }
            });
            leakDetection.Name = PoolName + "_leakDetection";
            leakDetection.IsBackground = true;
            leakDetection.Start();
        }

        /// <summary>
        /// 清除所有监测对象
        /// </summary>
        public void Clear()
        {

            PoolEntry poolEntry = null;
            while(true)
            {
                userQueue.TryDequeue(out poolEntry);
                if(userQueue.IsEmpty||poolEntry==null)
                {
                    break;
                }
            }
            //
            while (true)
            {
               
                maxLiveQueue.TryDequeue(out poolEntry);
                if (maxLiveQueue.IsEmpty || poolEntry == null)
                {
                    break;
                }
            }

            //
            while (true)
            {
               
                idleTimeQueue.TryDequeue(out poolEntry);
                if (idleTimeQueue.IsEmpty || poolEntry == null)
                {
                    break;
                }
            }
        }
       
        
        /// <summary>
        /// 关闭清除
        /// </summary>
        public void Stop()
        {
            Clear();
            IsStop = true;
        }
    }
}
