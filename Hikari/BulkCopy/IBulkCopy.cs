using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hikari.BulkCopy
{
   public interface IBulkCopy
    {
        void BulkCopy(DataTable dt);
        
    }
}
