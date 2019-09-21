using System;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.RabbitMQ.Abstractions
{
    public interface IConsumerResolver
    {

        Func<string, Task<bool>> ResolveConsumer(string eventName);

    }
}
