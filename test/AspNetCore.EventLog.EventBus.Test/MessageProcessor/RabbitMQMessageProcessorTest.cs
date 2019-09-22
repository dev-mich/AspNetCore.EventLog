using System;
using System.Threading.Tasks;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.RabbitMQ.Abstractions;
using AspNetCore.EventLog.RabbitMQ.Infrastructure;
using Moq;
using Xunit;

namespace AspNetCore.EventLog.EventBus.Test.MessageProcessor
{
    public class RabbitMQMessageProcessorTest: MessageProcessorTest
    {
        private Mock<IConsumerResolver> _consumerResolverMock;

        protected override IMessageProcessor InitMessageProcessor()
        {

            _consumerResolverMock = new Mock<IConsumerResolver>();

            return new RabbitMQMessageProcessor(ReceivedStoreMock.Object, _consumerResolverMock.Object);

        }


        [Fact]
        public Task TestUnresolvedConsumer()
        {
            var processor = InitMessageProcessor();

            _consumerResolverMock.Setup(s => s.ResolveConsumer("eventName")).Returns((Func<string, Task<bool>>)null);

            processor.PersistEvent(Guid.NewGuid(), "eventName", "content");

            return Assert.ThrowsAsync<ArgumentNullException>(() =>
                processor.Process("eventName", "content"));
        }

    }
}
