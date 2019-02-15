using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
        {
            IDbConnection connection = new SqlConnection();
         //   SqlBulkCopy copy = new SqlBulkCopy(connection);
            var ob=  Activator.CreateInstance(typeof(SqlBulkCopy), connection);
            int ss = 0;

        }
    }
}
