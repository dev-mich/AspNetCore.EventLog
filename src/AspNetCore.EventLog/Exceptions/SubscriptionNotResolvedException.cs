using System;

namespace AspNetCore.EventLog.Exceptions
{
    public class SubscriptionNotResolvedException : Exception
    {
        public SubscriptionNotResolvedException(string eventName) : base($"subscription not resolved for event {eventName}") { }
    }
}
