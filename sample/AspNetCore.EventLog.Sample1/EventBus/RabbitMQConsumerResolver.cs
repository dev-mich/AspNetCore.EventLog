using AspNetCore.EventLog.RabbitMQ.Abstractions;
using System;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Sample1.EventBus
{
    public class RabbitMQConsumerResolver : IConsumerResolver
    {
        public Func<string, Task<bool>> ResolveConsumer(string eventName)
        {
            return (content) =>
            {
                Console.WriteLine(content);
                return Task.FromResult(true);
            };
        }
    }
}
