using System;

namespace AspNetCore.EventLog.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// Throw this exception will mark the event dispatch as failed and throw the inner exception
    /// </summary>
    public class CriticalException: Exception
    {

        public CriticalException(Exception ex): base(string.Empty, ex) { }

    }
}
