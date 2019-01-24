using HikariAPI;
using System;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
          var orm=  ORMFactory.Create();
          var lst=  orm.Query<Product,Product>("SELECT * FROM \"Product\" ",new { ID=1 });
            Console.WriteLine("Hello World!");
        }
    }
}
