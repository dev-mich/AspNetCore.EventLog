
namespace AspNetCore.EventLog.RabbitMQ.Abstractions
{
    public interface IQueueResolver
    {
        string ResolveQueue(string eventName);
    }
}
