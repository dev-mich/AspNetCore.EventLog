using System;
using AspNetCore.EventLog.RabbitMQ;
using AspNetCore.EventLog.RabbitMQ.Abstractions;
using Moq;
using RabbitMQ.Client;

namespace AspNetCore.EventLog.EventBus.Test.Fixtures
{
    public class RabbitMQFixture
    {

        public Mock<IExchangeResolver> ExchangeResolverMock;
        public Mock<IServiceProvider> ServiceProviderMock;
        public RabbitMqEventBus RabbitMq;

        public RabbitMQFixture()
        {
            ServiceProviderMock = new Mock<IServiceProvider>();
            var connectionFactoryMock = new Mock<IConnectionFactory>();
            ExchangeResolverMock = new Mock<IExchangeResolver>();

            connectionFactoryMock.Setup(x => x.CreateConnection()).Returns(new Mock<IConnection>().Object);

            RabbitMq = new RabbitMqEventBus(ServiceProviderMock.Object, connectionFactoryMock.Object, ExchangeResolverMock.Object);
        }

    }
}
