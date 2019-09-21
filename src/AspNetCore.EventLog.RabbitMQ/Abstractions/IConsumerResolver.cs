using RabbitMQ.Client;

namespace AspNetCore.EventLog.RabbitMQ.Abstractions
{
    public interface IConsumerResolver
    {

        IAsyncBasicConsumer ResolveConsumer(string eventName);

    }
}
