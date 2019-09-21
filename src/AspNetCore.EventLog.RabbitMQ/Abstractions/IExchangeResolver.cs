namespace AspNetCore.EventLog.RabbitMQ.Abstractions
{
    public interface IExchangeResolver
    {

        string ResolveExchange(string eventName);

    }
}
