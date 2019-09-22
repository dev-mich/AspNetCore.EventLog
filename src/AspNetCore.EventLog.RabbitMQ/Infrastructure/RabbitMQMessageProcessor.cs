using System;
using System.Threading.Tasks;
using AspNetCore.EventLog.Abstractions.Persistence;
using AspNetCore.EventLog.Core.Infrastructure;
using AspNetCore.EventLog.RabbitMQ.Abstractions;

namespace AspNetCore.EventLog.RabbitMQ.Infrastructure
{
    public class RabbitMQMessageProcessor: MessageProcessor
    {
        private readonly IConsumerResolver _consumerResolver;

        public RabbitMQMessageProcessor(IReceivedStore receivedStore, IConsumerResolver consumerResolver) : base(receivedStore)
        {
            _consumerResolver = consumerResolver;
        }

        protected override Task<bool> Consume(string eventName, string content)
        {

            var consumer = _consumerResolver.ResolveConsumer(eventName);

            if (consumer == null)
                throw new ArgumentNullException(nameof(consumer));


            return consumer(content);

        }
    }
}
