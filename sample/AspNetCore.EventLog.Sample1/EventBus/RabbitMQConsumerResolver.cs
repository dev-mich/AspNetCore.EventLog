using AspNetCore.EventLog.RabbitMQ.Abstractions;
using System;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Sample1.EventBus
{
    public class RabbitMQConsumerResolver : IConsumerResolver
    {
        public Func<string, Task<bool>> ResolveConsumer(string eventName)
        {
            if (eventName.Equals("test.event"))
                return (content) =>
                {
                    Console.WriteLine(content);
                    return Task.FromResult(true);
                };

            return (content) => throw new Exception("fake consumer failed");
        }
    }
}
