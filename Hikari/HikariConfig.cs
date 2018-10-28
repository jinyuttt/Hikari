using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

/**
* 命名空间: Hikari 
* 类 名： HikariConfig
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace Hikari
{
    /// <summary>
    /// 功能描述    ：HikariConfig  配置
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/24 15:20:48 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/24 15:20:48 
    /// </summary>
    public class HikariConfig
    {
       
       
        private string connectString = null;
        private string driverDLL = null;
        private string driverDir = "DBDrivers";
        private string dbTypeXml = "DBType.xml";
        private long connectionTimeout = 3000;//池中取出时间
        private int idleTimeout = 600000;//10分钟,空闲时间
        private int maxLifetime = 1800000;//30分钟，最大生存时间
        private int maximumPoolSize = 10;//最大个数
        private int minimumIdle = 0;//最小空闲个数
        private string poolName = null;//池名称
        private byte initializationFailTimeout = 1;//初始化时间
        private string connectionInitSql = null;//初始化连接测试SQL
        private long validationTimeout = 5000;//验证时间，现在无用
        private long leakDetectionThreshold = 0;//离开池中时间
        private string logConfig = "";
        private int destroyInterval = 600000;//销毁驱动连接的时间间隔
        private static int pool_number = 0;
        private const string prefix = "HikariPool-";
      
        /// <summary>
        /// 
        /// 数据库连接字符串
        /// 
        /// </sumsry>
        public string ConnectString { get { return connectString; } set { connectString = value; } }

        /// <summary>
        /// 等待池中连接的最大毫秒数
        /// 默认：3秒（3000毫秒）
        /// </summary>
        public long ConnectionTimeout { get { return connectionTimeout; } set { connectionTimeout = value; } }

        /// <summary>
        /// 控制允许连接在池中空闲的最长时间
        /// 此设置仅在minimumIdle定义为小于时才适用maximumPoolSize
        /// 默认：10分钟（600000毫秒）
        /// </summary>
        public int IdleTimeout { get { return idleTimeout; } set { idleTimeout = value; } }

        /// <summary>
        /// 控制池中连接的最长生命周期
        /// 使用中的连接永远不会退役，只有当它关闭时才会被删除
        /// 默认：30分钟(1800000毫秒）
        /// </summary>
        public int MaxLifetime { get { return maxLifetime; } set { maxLifetime = value; } }

      
        /// <summary>
        /// 在池中维护的最小空闲连接数
        /// 空闲连接低于此值并且池中的总连接数小于maximumPoolSize，
        /// 则HikariCP将尽最大努力快速有效地添加其他连接
        /// 为了获得最高性能和对峰值需求的响应，
        /// 我们建议不要设置此值，而是允许HikariCP充当固定大小的连接池
        /// 默认值：与maximumPoolSize相同
        /// 如果超过逻辑CPU线程2倍，则设置为2倍
        /// 
        /// </summary>
        public int MinimumIdle { get { return minimumIdle; } set { minimumIdle = value; } }

        /// <summary>
        /// 控制允许池到达的最大大小，包括空闲和正在使用的连接
        /// 默认值：10
        /// 
        /// </summary>
        public int MaximumPoolSize { get { return maximumPoolSize; } set { maximumPoolSize = value; } }

       
       
        /// <summary>
        /// 池名称
        /// 默认：自动生成
        /// </summary>
        public string PoolName { get { return poolName; } set { poolName = value; } }

        /// <summary>
        /// 如果池无法成功初始化连接，则此属性控制池是否“快速失败”。
        /// 任何正数都被认为是尝试获取初始连接的毫秒数; 在此期间，应用程序线程将被阻止。
        /// 如果在此超时发生之前无法获取连接，则将引发异常。
        /// 此超时被应用后的connectionTimeout 期。如果值为零（0），HikariCP将尝试获取并验证连接。
        /// 如果获得连接但验证失败，则将引发异常并且池未启动。
        /// 但是，如果无法获得连接，则池将启动，但稍后获取连接的努力可能会失败。
        /// 小于零的值将绕过任何初始连接尝试，并且池将在尝试在后台获取连接时立即启动。
        /// 因此，稍后获得连接的努力可能失败。
        /// 默认值：1
        /// </summary>
        public byte InitializationFailTimeout { get { return initializationFailTimeout; } set { initializationFailTimeout = value; } }
      
        /// <summary>
        /// 此属性设置一个SQL语句，该语句将在每次创建新连接之后执行，然后再将其添加到池中
        /// </summary>
        public string ConnectionInitSql { get { return connectionInitSql; } set { connectionInitSql = value; } }

       
        /// <summary>
        /// 此属性控制连接测试活动的最长时间。
        /// 该值必须小于connectionTimeout。
        /// 最低可接受的验证超时为250毫秒。
        /// 默认值：5000
        /// </summary>
        public long ValidationTimeout { get { return validationTimeout; } set { validationTimeout = value; } }

        /// <summary>
        /// 此属性控制在记录消息之前连接可以离开池的时间量，指示可能的连接泄漏
        /// 值为0表示禁用泄漏检测。
        /// 启用泄漏检测的最低可接受值是2000（2秒）。
        /// 默认值：0
        /// </summary>
        public long LeakDetectionThreshold { get { return leakDetectionThreshold; } set { leakDetectionThreshold = value; } }

     
        /// <summary>
        /// 驱动dll路径
        /// </summary>
        public string DriverDLL { get { return driverDLL; } set { driverDLL = value; } }

        /// <summary>
        /// 驱动目录
        /// 默认：DBDrivers
        /// </summary>
        public string DriverDir { get { return driverDir; } set { driverDir = value; } }



        /// <summary>
        /// 全局配置
        /// 默认：DBType.xml
        /// </summary>
        public string DBTypeXml { get { return dbTypeXml; } set { dbTypeXml = value; } }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DBType { get; set; }

        /// <summary>
        /// 日志配置文件
        /// </summary>
        public string LogConfig { get { return logConfig; } set { logConfig = value; LogConfiguration(); } }

        /// <summary>
        /// 销毁驱动连接的时间间隔
        /// 默认：10分钟(600000毫秒)
        /// </summary>
        public int DestroyInterval { get { return destroyInterval; } set { destroyInterval = value; } }
        /// <summary>
        /// 获取数据
        /// </summary>
        public void Validate()
        {
            if (poolName == null)
            {
                //输出一个名称
                poolName = GeneratePoolName();
            }
            connectionInitSql = GetNullIfEmpty(connectionInitSql);
            connectString = GetNullIfEmpty(connectString);
             if (connectString != null&&driverDLL!=null)
            {
                // ok
                //需要连接字符串和DLL名称
            }
            else if(connectString!=null&&DBType!=null)
            {
                //ok
                //需要连接字符串和DBType项
            }
            else if (driverDLL != null)
            {
                //说明没有连接字符串
                Logger.Singleton.ErrorFormat("{0} - connectString is required with driverDLL.", poolName);
                throw new Exception("connectString is required ");
            }
            else if(DBType==null)
            {
                //说明没有driverDLL，没有DBType
                Logger.Singleton.ErrorFormat("{0} - DBType or connectString is required.", poolName);
                throw new Exception("DBType or connectString is required.");
            }
            else
            {
                //说明全部没有
                Logger.Singleton.ErrorFormat("{0} - DBType or driverDLL and connectString is required.", poolName);
                throw new Exception("DBType or driverDLL or connectString is required.");
            }
            // validateNumerics();

        }

       /// <summary>
       /// 配置日志
       /// </summary>
        private void LogConfiguration()
        {
            Logger.Singleton.LogConfiguration(logConfig);
        }

        private string GetNullIfEmpty(string catalog)
        {
            return catalog;
        }

        /// <summary>
        /// 名称
        /// </summary>
        /// <returns></returns>
        private string GeneratePoolName()
        {
            // Pool number is global to the VM to avoid overlapping pool numbers in classloader scoped environments
            return prefix + Interlocked.Increment(ref pool_number);
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="other"></param>
        public void CopyStateTo(HikariConfig other)
        {
            var propertys = typeof(HikariConfig).GetProperties();
            foreach(var property in propertys)
            {
                property.SetValue(other, property.GetValue(this));
            }
           // other.isSealed = false;
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadConfig(string filePath)
        {
            if(!File.Exists(filePath))
            {
                Logger.Singleton.Error("没有找到配置文件");
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            using (StreamReader rd = new StreamReader(filePath))
            {
                string strLine = null;
                while ((strLine = rd.ReadLine()) != null)
                {
                    
                    string[] cof = strLine.Split('=');
                    if (cof.Length == 2)
                    {
                        dic[cof[0].Trim().ToLower()] = cof[1] == null ? "" : cof[1].Trim();
                    }
                    else if(cof.Length>2)
                    {
                        StringBuilder sbr = new StringBuilder();
                        for(int i=1;i<cof.Length;i++)
                        {
                            sbr.Append(cof[i]);
                        }
                        dic[cof[0].Trim().ToLower()] = sbr.ToString();
                    }
                }
            }
            //
            if(dic.Count>0)
            {
                var propertys = typeof(HikariConfig).GetProperties();
                foreach (var property in propertys)
                {
                    string value = "";
                   if(dic.TryGetValue(property.Name.ToLower(),out value))
                    {
                        //
                        try
                        {
                            property.SetValue(this, Convert.ChangeType(value, property.PropertyType), null);//类型转换。
                        }
                        catch(Exception ex)
                        {
                            Logger.Singleton.Error(property.Name + "配置属性赋值转换错误," + ex.Message);
                        }
                    }
                }
               
            }
        }
    }
}
