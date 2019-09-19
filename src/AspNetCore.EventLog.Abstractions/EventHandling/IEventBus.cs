
namespace AspNetCore.EventLog.Abstractions.EventHandling
{
    public interface IEventBus
    {

        void Publish(string eventName, string content);

        void Subscribe(string eventName);

    }
}
