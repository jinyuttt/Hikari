using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hikari.Integration.Models.Emit;

namespace ConsoleApp2
{
    /// <summary>
    /// 数据库扩展测试
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
            for(int i=1;i<1000000;i++)
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
            Console.WriteLine(lst.Count+","+watch.ElapsedMilliseconds);
            Console.ReadKey();

        }
    }
}
