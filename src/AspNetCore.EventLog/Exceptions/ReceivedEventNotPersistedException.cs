using System;

namespace AspNetCore.EventLog.Exceptions
{
    public class ReceivedEventNotPersistedException : Exception
    {

        public ReceivedEventNotPersistedException(string eventName): base($"failed to store event {eventName}") { }

    }
}
