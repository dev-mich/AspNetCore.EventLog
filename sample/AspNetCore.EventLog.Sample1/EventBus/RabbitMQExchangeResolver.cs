using AspNetCore.EventLog.RabbitMQ.Abstractions;

namespace AspNetCore.EventLog.Sample1.EventBus
{
    public class RabbitMQExchangeResolver: IExchangeResolver
    {
        public string ResolveExchange(string eventName)
        {
            if (eventName.Contains("reply"))
            {
                return "";
            }

            return "test";
        }
    }
}
