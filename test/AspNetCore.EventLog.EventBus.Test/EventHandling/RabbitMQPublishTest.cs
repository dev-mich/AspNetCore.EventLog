using System;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.RabbitMQ;
using AspNetCore.EventLog.RabbitMQ.Abstractions;
using Moq;
using RabbitMQ.Client;
using Xunit;

namespace AspNetCore.EventLog.EventBus.Test.EventHandling
{
    public class RabbitMQPublishTest: PublishTest
    {
        private Mock<IExchangeResolver> _exchangeResolverMock;

        protected override IEventBus InitEventBus()
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            var connectionFactoryMock = new Mock<IConnectionFactory>();
            _exchangeResolverMock = new Mock<IExchangeResolver>();

            connectionFactoryMock.Setup(x => x.CreateConnection()).Returns(new Mock<IConnection>().Object);

            return new RabbitMqEventBus(serviceProviderMock.Object, connectionFactoryMock.Object, _exchangeResolverMock.Object);
        }

        [Fact]
        public void TestUnresolvedExchange()
        {

            var bus = InitEventBus();

            _exchangeResolverMock.Setup(e => e.ResolveExchange(It.IsAny<string>())).Returns((string)null);

            Assert.Throws<ArgumentNullException>(() => bus.Publish("event", "content"));

        }
    }
}
