using Hikari.Integration.Models.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Hikari.Integration.Models;
namespace ConsoleApp3
{

    /// <summary>
    /// 测试数据库扩展 .net core
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            DataTable dt = new DataTable();
            Random random = new Random();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Age", typeof(int));
            for (int i = 1; i < 65000; i++)
            {
                var row = dt.NewRow();
                row[0] = i;
                row[1] = "jy" + random.Next();
                row[2] = random.Next(10, 50);
                dt.Rows.Add(row);
            }
            Stopwatch watch = new Stopwatch();
            watch.Reset();
            watch.Start();
            List<Person> lst = dt.ToEntityEmitList<Person>();
            watch.Stop();
            Stopwatch watchDD = new Stopwatch();
            watchDD.Start();
            var dd= lst.FromEntity();
            watchDD.Stop();
            Console.WriteLine(lst.Count + "," + watch.ElapsedMilliseconds+","+watchDD.ElapsedMilliseconds);
            Console.ReadKey();
        }
    }
}
