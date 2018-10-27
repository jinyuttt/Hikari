using log4net.Core;
using System;
using System.Collections.Generic;
using System.Data;

/**
* 命名空间: Hikari 
* 类 名： ProxyConnection
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace Hikari
{
    /// <summary>
    /// 功能描述    ：ProxyConnection  
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/24 18:09:50 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/24 18:09:50 
    /// </summary>
    public abstract class ProxyConnection : IDbConnection
    {
        static int DIRTY_BIT_READONLY = 0b000001;
        static int DIRTY_BIT_AUTOCOMMIT = 0b000010;
        static int DIRTY_BIT_ISOLATION = 0b000100;
        static int DIRTY_BIT_CATALOG = 0b001000;
        static int DIRTY_BIT_NETTIMEOUT = 0b010000;
        static int DIRTY_BIT_SCHEMA = 0b100000;
       
        private static ISet<string> ERROR_STATES;
        private static ISet<int> ERROR_CODES;
        private long lastAccess=0;
        protected IDbConnection delegateCon = null;
        private  PoolEntry poolEntry = null;
        public IDbTransaction BeginTransaction()
        {
           return delegateCon.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return delegateCon.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            delegateCon.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            poolEntry.Recycle(DateTime.Now.Ticks);
        }

        public IDbCommand CreateCommand()
        {
           return delegateCon.CreateCommand();
        }

        public void Open()
        {
            delegateCon.Open();
        }

        public void Dispose()
        {
            this.Close();
        }



        public string ConnectionString { get { return delegateCon.ConnectionString; } set { delegateCon.ConnectionString = value; } }

        public int ConnectionTimeout { get { return delegateCon.ConnectionTimeout; } }

        public string Database { get { return delegateCon.Database; }  }

        public ConnectionState State { get { return delegateCon.State; } }
        public ProxyConnection(PoolEntry poolEntry,  IDbConnection connection,  long now)
        {
            try
            {
                this.poolEntry = poolEntry;
                this.delegateCon = connection;
                this.lastAccess = now;
               // Init();
            }
            catch (Exception ex)
            {
                throw new Exception("获取失败56:" + ex.Message);
            }
        }

        private void Init()
        {
            ERROR_STATES = new HashSet<string>();
            ERROR_STATES.Add("0A000"); // FEATURE UNSUPPORTED
            ERROR_STATES.Add("57P01"); // ADMIN SHUTDOWN
            ERROR_STATES.Add("57P02"); // CRASH SHUTDOWN
            ERROR_STATES.Add("57P03"); // CANNOT CONNECT NOW
            ERROR_STATES.Add("01002"); // SQL92 disconnect error
            ERROR_STATES.Add("JZ0C0"); // Sybase disconnect error
            ERROR_STATES.Add("JZ0C1"); // Sybase disconnect error

            ERROR_CODES = new HashSet<int>();
            ERROR_CODES.Add(500150);
            ERROR_CODES.Add(2399);
        }
       

       

       

        // ***********************************************************************
        //                          Internal methods
        // ***********************************************************************

        public void ShutDown()
        {
            delegateCon.Close();
            delegateCon.Dispose();
        }
    }

}

