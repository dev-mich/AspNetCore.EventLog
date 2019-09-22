using System;


namespace AspNetCore.EventLog.Core.Exceptions
{
    public class PersistenceException : Exception
    {
        public PersistenceException(string message): base(message) { }
    }
}
