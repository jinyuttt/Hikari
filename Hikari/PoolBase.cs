using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hikari
{

    /// <summary>
    /// 创建驱动连接
    /// </summary>
    public abstract class PoolBase
    {
      
        protected static int MAX_PERMITS = 10000;
        protected HikariConfig config;
        protected string poolName;
        protected long connectionTimeout;
        protected int validationTimeout;
        protected string dllPath = "";//dll路径
        protected  int size = 0;//生成的连接数量
        protected int entryid = 0;//ID生成

        /// <summary>
        /// 已经创建的数据
        /// </summary>
        public int Size { get { return size; } }

        public PoolBase(HikariConfig config)
        {
            this.config = config;
            this.poolName = config.PoolName;
            this.connectionTimeout = config.ConnectionTimeout;
            this.validationTimeout =(int) config.ValidationTimeout;
        }
       

        /// <summary>
        /// 创建池中数据对象
        /// 
        /// </summary>
        /// <returns></returns>
        protected PoolEntry NewPoolEntry()
        {
            PoolEntry poolEntry= new PoolEntry(NewConnection(), this);
            if(poolEntry!=null)
            {
                poolEntry.ID=Interlocked.Increment(ref entryid);
                Interlocked.Increment(ref size);
            }
            return poolEntry;
        }

        /// <summary>
        /// 关闭驱动连接
        /// 按照设计，只有连接池能够操作驱动连接
        /// 
        /// </summary>
        /// <param name="connection"></param>
        protected void CloseConnection(IDbConnection connection)
        {
            if(connection!=null)
            {
                connection.Close();
                connection.Dispose();
                Interlocked.Decrement(ref size);
            }
        }

        /// <summary>
        /// 创建驱动的连接
        /// </summary>
        /// <returns></returns>
        private IDbConnection NewConnection()
        {
            long start = DateTime.Now.Ticks;
            IDbConnection connection = null;
            try
            {
                if(string.IsNullOrEmpty(dllPath))
                {
                    dllPath = Path.Combine(config.DriverDir, config.DriverDLL);
                }
                connection = ProxyLoad.GetConnection(dllPath);
                if (connection == null)
                {
                    throw new Exception("DataSource returned null unexpectedly");
                }
                SetupConnection(connection);
                return connection;
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
               

            }
        }

       /// <summary>
       /// 测试连接及设置
       /// </summary>
       /// <param name="connection"></param>
        private void SetupConnection(IDbConnection connection)
        {
            try
            {

                connection.ConnectionString = config.ConnectString;
                ExecuteSql(connection, config.ConnectionInitSql, true);

                SetNetworkTimeout(connection, config.ConnectionTimeout);
            }
            catch (SQLException e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 连接验证
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="v"></param>
        private void ExecuteSql(IDbConnection connection, string sql, bool v)
        {
            connection.Open();
            if (string.IsNullOrEmpty(sql))
            {
                return;
            }
            var cts = new CancellationTokenSource(validationTimeout);
           var cancell=  cts.Token.Register(() => Logger.Singleton.Warn("当前连接执行测试超时,SQL:"+sql));
           Task result=  Task.Factory.StartNew(() =>
            {
               
                IDbCommand command = connection.CreateCommand();
                command.CommandText = sql;
                int r = command.ExecuteNonQuery();
                command.Dispose();

            }, cts.Token
                );
            result.Wait(validationTimeout,cts.Token);
            cancell.Dispose();
            cts.Dispose();

        }

       /// <summary>
       /// 验证网络
       /// c#全部放在了驱动上面
       /// 这里只是演示一个流程
       /// </summary>
       /// <param name="connection"></param>
       /// <param name="validationTimeout"></param>
        private void SetNetworkTimeout(IDbConnection connection, long validationTimeout)
        {

        }
        public override string ToString()
        {
            return poolName;
        }
        private void SetLoginTimeout(IDbConnection connection)
        {

        }

        #region 数据库主要对象
        public IDbCommand  GetDbCommand()
        {
            return ProxyLoad.GetDbCommand(dllPath);
        }

        public IDbDataParameter GetDataParameter()
        {
            return ProxyLoad.GetDataParameter(dllPath);
        }

        public IDbDataAdapter GetDataAdapter()
        {
            return ProxyLoad.GetDataAdapter(dllPath);
        }
        #endregion
    }
}