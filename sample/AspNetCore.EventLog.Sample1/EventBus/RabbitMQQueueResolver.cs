using AspNetCore.EventLog.RabbitMQ.Abstractions;

namespace AspNetCore.EventLog.Sample1.EventBus
{
    public class RabbitMQQueueResolver : IQueueResolver
    {
        public string ResolveQueue(string eventName)
        {
            if (eventName.Contains("reply"))
            {
                return eventName;
            }

            return "test.queue";
        }
    }
}
