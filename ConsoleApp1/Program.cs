using System;
using System.Threading.Tasks;
using Hikari;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            TestConnect();
          //  TestBag();
            Console.Read();

        }
      
        private static void TestBag()
        {

            ConcurrentList<PoolEntry> connectionList = new ConcurrentList<PoolEntry>();
            int num = 100;
            Task[] tasks = new Task[num];
            Task[] read = new Task[num];
            DateTime start = DateTime.Now;
            //写入
            for (int i=0;i<num;i++)
            {
              var r= Task.Factory.StartNew(() => {
                    for (int j = 0; j < 10000; j++)
                    {
                        PoolEntry poolEntry = new PoolEntry(null, null);
                        connectionList.Push(poolEntry);
                    }
                });
                tasks[i] = r;
            }
           
            //读取
            for (int i = 0; i < num; i++)
            {
                var r = Task.Factory.StartNew(() =>
                {
                    PoolEntry poolEntry = null;
                    while (!connectionList.IsEmpty)
                    {
                        connectionList.TryPop(out poolEntry);
                    }
                });
                read[i] = r;
            }
            Task.WaitAll(read);
            Task.WaitAll(tasks);
            
            Console.WriteLine("时间:" + (DateTime.Now - start).TotalSeconds);

        }
        private static void TestConnect()
        {
          
            HikariConfig hikariConfig = new HikariConfig();
            hikariConfig.DBType = "PostgreSQL";
            hikariConfig.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 1234; Database = postgres;Pooling=true; ";
            //hikariConfig.DriverDir = "DBDrivers";
            //hikariConfig.DriverDLL = "XXXX.dll";
            //hikariConfig.DBTypeXml = "DBType.xml";
            HikariDataSource hikariDataSource = new HikariDataSource(hikariConfig);
            //
            //hikariConfig.LoadConfig("Hikari.txt");
            // hikariDataSource.LoadConfig();

            //

            //HikariDataSource hikariDataSource = new HikariDataSource();
            //hikariDataSource.LoadConfig("Hikari.txt");
            //hikariDataSource.DBType = "PostgreSQL";
            //hikariDataSource.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 1234; Database = postgres;Pooling=true; ";
            //hikariDataSource.DriverDir = "DBDrivers";
            //hikariDataSource.DriverDLL = "XXXX.dll";
            //hikariDataSource.DBTypeXml = "DBType.xml";
            int num = 1000;
            Task[] tasks = new Task[num];
            DateTime start = DateTime.Now;
            for (int i = 0; i < num; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    var connection = hikariDataSource.GetConnection();
                    if (connection != null)
                    {
                        var cmd = connection.CreateCommand();
                        cmd.CommandText = "select * from student";
                        var rd = cmd.ExecuteReader();
                        int datanum = 0;
                        while (rd.Read())
                        {
                            rd.GetInt32(0);
                            rd.GetString(1);
                            rd.GetInt32(2);
                            datanum++;
                        }
                        rd.Close();
                        cmd.Dispose();
                        connection.Close();
                        Console.WriteLine(datanum);
                    }

                });

            }
            Task.WaitAll(tasks);
            Console.WriteLine("时间:" + (DateTime.Now - start).TotalSeconds);
        }
    }
}
