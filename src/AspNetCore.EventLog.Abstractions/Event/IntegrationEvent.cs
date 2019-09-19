
namespace AspNetCore.EventLog.Abstractions.Event
{
    public abstract class IntegrationEvent
    {
        public string EventName { get; }

        protected IntegrationEvent(string eventName)
        {
            EventName = eventName;
        }

    }
}
