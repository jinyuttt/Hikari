using System;
using System.Runtime.Serialization;

namespace Hikari
{
    [Serializable]
    internal class SQLException : Exception
    {
        public SQLException()
        {
        }

        public SQLException(string message) : base(message)
        {
        }

        public SQLException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SQLException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal string GetSQLState()
        {
            throw new NotImplementedException();
        }

        internal int GetErrorCode()
        {
            throw new NotImplementedException();
        }

        internal SQLException GetNextException()
        {
            throw new NotImplementedException();
        }
    }
}