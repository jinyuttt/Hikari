using System;
using System.Collections.Generic;
using System.Text;

namespace Hikari.Integration.Models
{
  public  interface IORM
    {
        List<T> Query<T, P>(string sql, P param=default(P)) where T:new();

        int Execute<P>(string sql, P param = default(P));

        bool SqlBulkCopy<P>(string sql, List<P> lst = null);

        object ExecuteScalar<T, P>(string sql, P param = default(P));
    }
}
