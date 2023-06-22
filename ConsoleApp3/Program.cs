using Hikari;
using Hikari.Manager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;

namespace ConsoleApp3
{

    /// <summary>
    /// 数据库连接池测试
    /// </summary>
    class Program
    {
        private static string connstr = "server=127.0.0.1;database=mystudy;username=root;password=123456;";
        static void Main(string[] args)
        {

            //Log.Logger = new LoggerConfiguration()
            //.MinimumLevel.Debug()
            //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //.Enrich.FromLogContext()
            //.WriteTo.Console()
            //.CreateLogger();

            try
            {
              //  Log.Information("Starting host");
                BuildHost(args);

            }
            catch (Exception ex)
            {
              //  Log.Fatal(ex, "Host terminated unexpectedly");

            }
            //finally
            //{
            //    Log.CloseAndFlush();
            //}

            Npgsql.NpgsqlParameter ss = new Npgsql.NpgsqlParameter();
            ss.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Oidvector;

            //  Console.WriteLine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());
            // TestQuery();
            //  TestManager();
            TestConnect();
            //  TestBag();

            Console.Read();

        }

        public static IHost BuildHost(string[] args)
        {
            return new HostBuilder()
            .ConfigureServices(services => services.AddSingleton<IHostedService, PrintTimeService>())
            .UseSerilog((context, configuration) =>
            {
                configuration
                .MinimumLevel.Information()
                // 日志调用类命名空间如果以 Microsoft 开头，覆盖日志输出最小级别为 Information
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // 配置日志输出到控制台
                .WriteTo.Console();
                // 配置日志输出到文件，文件输出到当前项目的 logs 目录下
                // 日记的生成周期为每天
               // .WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day)
                // 创建 logger
               // .CreateLogger();
            }) // <- Add this line
            .Build();
        }
   

    //private static void TestBag()
    //{

    //    ConcurrentList<PoolEntry> connectionList = new ConcurrentList<PoolEntry>();
    //    int num = 100;
    //    Task[] tasks = new Task[num];
    //    Task[] read = new Task[num];
    //    DateTime start = DateTime.Now;
    //    //写入
    //    for (int i=0;i<num;i++)
    //    {
    //      var r= Task.Factory.StartNew(() => {
    //            for (int j = 0; j < 10000; j++)
    //            {
    //                PoolEntry poolEntry = new PoolEntry(null, null);
    //                connectionList.Push(poolEntry);
    //            }
    //        });
    //        tasks[i] = r;
    //    }

    //    //读取
    //    for (int i = 0; i < num; i++)
    //    {
    //        var r = Task.Factory.StartNew(() =>
    //        {
    //            PoolEntry poolEntry = null;
    //            while (!connectionList.IsEmpty)
    //            {
    //                connectionList.TryPop(out poolEntry);
    //            }
    //        });
    //        read[i] = r;
    //    }
    //    Task.WaitAll(read);
    //    Task.WaitAll(tasks);

    //    Console.WriteLine("时间:" + (DateTime.Now - start).TotalSeconds);

    //}

    private static void TestConnect()
        {

            HikariConfig hikariConfig = new HikariConfig();
            // hikariConfig.DBType = "PostgreSQL";
            //  hikariConfig.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 123456; Database = postgres;Pooling=true; ";
            hikariConfig.DBType = "MySql";
            hikariConfig.ConnectString = connstr;
            //hikariConfig.DriverDir = "DBDrivers";
            //hikariConfig.DriverDLL = "XXXX.dll";
            HikariDataSource hikariDataSource=null;
            try
            {
                //hikariConfig.DBTypeXml = "DBType.xml";
                 hikariDataSource = new HikariDataSource(hikariConfig);

                Dictionary<string, SqlValue> map = new Dictionary<string, SqlValue>();
                Dictionary<string, object> mapkk = new Dictionary<string, object>();
                Dictionary<string, object> outmap = new Dictionary<string, object>();
                Dictionary<string, SqlValue> outmapkk = new Dictionary<string, SqlValue>();
                // map.Add("p", new SqlValue() { Type= "IPAddress", Value= pAddress });
                //map.Add("m", new SqlValue() { Type="uint[]", Value=ss});

                map.Add("CartID", new SqlValue() { Type="String", Value="32"});
                //  mapkk.Add("categoryName", "测试");
                mapkk.Add("CartID", 32);
                outmap.Add("ItemCount", 0);
                outmapkk.Add("ItemCount", new SqlValue() { Type="Int32",Value=""});
                //mapkk.Add("m", 32);
                hikariDataSource.ExecuteStoredProcedure("ShoppingCartItemCount", map,outmapkk);
               var ds= hikariDataSource.ExecuteStoredProcedureWithValue("ShoppingCartItemCount", map,outmapkk);
                int d = 0;

                //Dictionary<string, SqlValue> map = new Dictionary<string, SqlValue>();
                //Dictionary<string, object> mapkk = new Dictionary<string, object>();
                //uint[] ss = new uint[3] {1,2, 3};
                //IPAddress pAddress =  IPAddress.Parse("127.0.0.1");
                //map.Add("p", new SqlValue() { Type= "IPAddress", Value= pAddress });
                //map.Add("m", new SqlValue() { Type="uint[]", Value=ss});
                //mapkk.Add("p", pAddress);
                //mapkk.Add("m", 32);
                //hikariDataSource.ExecuteUpdate("insert into test(temp,kk)values(@p,@m)", map);

            }
            catch (Exception ex)
            {
                
            }
            //
            //hikariConfig.LoadConfig("Hikari.txt");
            // hikariDataSource.LoadConfig();

            //


            //
            var connection1 = hikariDataSource.GetConnection();
            // var bulk= hikariDataSource.GetBulkCopy();
            //DataTable dt = new DataTable();
            //dt.TableName = "\"Student\"";
            //bulk.BulkCopy(dt);

            
            if (connection1 != null)
            {
                var cmd = connection1.CreateCommand();
                cmd.CommandText = "select * from \"Person\"";
                cmd.CommandText = "insert into test(temp)values(@p)";


                var p=  cmd.CreateParameter();
                  cmd.CommandText = "COPY \"Student\" FROM 'd:/test.csv'  WITH CSV  HEADER";
                //  cmd.ExecuteNonQuery();

               // var rd = cmd.ExecuteReader();
                //int datanum = 0;
                //string data = "";
                //while (rd.Read())
                //{
                //    data += "ID:" + rd.GetInt32(0);
                //    data += ",Name:" + rd.GetString(1);
                //    Console.WriteLine(data);
                //    datanum++;
                //}
              //  rd.Close();
                //  cmd.Dispose();
                //  connection1.Close();
                // Console.WriteLine(datanum);
            }
            //
            //    int num = 1000;
            //Task[] tasks = new Task[num];
            //DateTime start = DateTime.Now;
            //for (int i = 0; i < num; i++)
            //{
            //    tasks[i] = Task.Factory.StartNew(() =>
            //    {
            //        var connection = hikariDataSource.GetConnection();
            //        if (connection != null)
            //        {
            //            var cmd = connection.CreateCommand();
            //            cmd.CommandText = "select * from student";
            //            var rd = cmd.ExecuteReader();
            //            int datanum = 0;
            //            string data = "";
            //            while (rd.Read())
            //            {
            //                data+="ID:"+rd.GetInt32(0);
            //                data+=",Name:"+ rd.GetString(1);
            //                Console.WriteLine(data);
            //                datanum++;
            //            }
            //            rd.Close();
            //            cmd.Dispose();
            //            connection.Close();
            //            Console.WriteLine(datanum);
            //        }

            //    });

            //}
            //Task.WaitAll(tasks);
            //Console.WriteLine("时间:" + (DateTime.Now - start).TotalSeconds);
        }

        private static void TestQuery()
        {
            string sql = "select * from  person where id=@ID";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["ID"] = 1;
            HikariConfig hikariConfig = new HikariConfig();
            hikariConfig.DBType = "PostgreSQL";
            hikariConfig.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 1234; Database = postgres;Pooling=true; ";
            HikariDataSource hikariDataSource = new HikariDataSource(hikariConfig);
            var ds = hikariDataSource.ExecuteQuery(sql, dic);
        }
        private static void TestManager()
        {
            string sql = "select * from  person";
            var ds = ManagerPool.Singleton.ExecuteQuery(sql);
            var dt = ds.Tables[0];
            sql = "insert into person(id,name)values(1,'jinyu')";
            int r = ManagerPool.Singleton.ExecuteUpdate(sql);

        }



    }
}
