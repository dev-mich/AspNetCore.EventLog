
namespace AspNetCore.EventLog.Interfaces
{
    public interface IReceiverService
    {

        void Subscribe<TEvent>(string eventName) where TEvent : IIntegrationEvent;

        void Unsubscribe(string eventName);

    }
}
