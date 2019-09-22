

namespace AspNetCore.EventLog.Abstractions.EventHandling
{
    public interface IEventBus
    {

        void Publish(string eventName, string content);

        void Subscribe<TEvent>(string eventName) where TEvent : IIntegrationEvent;


    }
}
