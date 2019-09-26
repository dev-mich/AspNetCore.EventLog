

namespace AspNetCore.EventLog.Interfaces
{
    public interface IEventBus
    {

        void Publish(string eventName, string content);

        void Subscribe(string eventName);


    }
}
