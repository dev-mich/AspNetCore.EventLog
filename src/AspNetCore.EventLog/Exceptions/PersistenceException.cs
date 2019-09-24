using System;

namespace AspNetCore.EventLog.Exceptions
{
    public class PersistenceException : Exception
    {
        public PersistenceException(string message): base(message) { }
    }
}
