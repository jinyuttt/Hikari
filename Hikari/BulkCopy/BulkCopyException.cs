using System;

namespace Hikari.BulkCopy
{

    /// <summary>
    /// BulkCopy异常
    /// </summary>
    public class BulkCopyException : Exception
    {
        public BulkCopyException(string message) : base(message)
        {
        }
    }
}
