using System;
using System.Collections.Generic;
using Hikari;
using Hikari.Manager;
using HikariAPI;

namespace ConsoleApp1
{

    /// <summary>
    /// 数据库连接池测试
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //  Console.WriteLine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());
            // TestQuery();
            //  TestManager();
            // TestConnect();
            //  TestBag();
            TestXML();
            Console.Read();

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
              //  cmd.CommandText = "COPY \"Student\" FROM 'd:/test.csv'  WITH CSV  HEADER";
              //  cmd.ExecuteNonQuery();
              
                var rd = cmd.ExecuteReader();
                //int datanum = 0;
                //string data = "";
                //while (rd.Read())
                //{
                //    data += "ID:" + rd.GetInt32(0);
                //    data += ",Name:" + rd.GetString(1);
                //    Console.WriteLine(data);
                //    datanum++;
                //}
                rd.Close();
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
            string sql= "select * from  person where id=@ID";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["ID"] = 1;
              HikariConfig hikariConfig = new HikariConfig();
            hikariConfig.DBType = "PostgreSQL";
            hikariConfig.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 1234; Database = postgres;Pooling=true; ";
            HikariDataSource hikariDataSource = new HikariDataSource(hikariConfig);
            var ds= hikariDataSource.ExecuteQuery(sql,dic);
        }
        private static void TestManager()
        {
            string sql = "select * from  person";
            var ds= ManagerPool.Singleton.ExecuteQuery(sql);
            var dt = ds.Tables[0];
            sql = "insert into person(id,name)values(1,'jinyu')";
            int r=ManagerPool.Singleton.ExecuteUpdate(sql);

        }


        private static void  TestXML()
        {
            Configuration configuration = new Configuration("MySql_Sql.xml");
            configuration.ReadAsync("UserModel/UserInfo");

        }
    }
}
