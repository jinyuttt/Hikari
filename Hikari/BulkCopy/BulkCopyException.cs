using System;
using System.Collections.Generic;
using System.Text;

namespace Hikari.BulkCopy
{
    public class BulkCopyException : Exception
    {
        public BulkCopyException(string message) : base(message)
        {
        }
    }
}
