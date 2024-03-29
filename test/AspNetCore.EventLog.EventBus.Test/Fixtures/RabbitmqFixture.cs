﻿using System;
using AspNetCore.EventLog.RabbitMQ;
using AspNetCore.EventLog.RabbitMQ.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;

namespace AspNetCore.EventLog.EventBus.Test.Fixtures
{
    public class RabbitMQFixture : IDisposable
    {

        public Mock<IExchangeResolver> ExchangeResolverMock;
        public Mock<IServiceProvider> ServiceProviderMock;
        public RabbitMqEventBus RabbitMq;

        public RabbitMQFixture()
        {
            InitRabbitMQ();
        }


        private void InitRabbitMQ()
        {
            var loggerMock = new Mock<ILogger<RabbitMqEventBus>>();
            ServiceProviderMock = new Mock<IServiceProvider>();
            var connectionFactoryMock = new Mock<IConnectionFactory>();
            ExchangeResolverMock = new Mock<IExchangeResolver>();

            connectionFactoryMock.Setup(x => x.CreateConnection()).Returns(new Mock<IConnection>().Object);

            RabbitMq = new RabbitMqEventBus(loggerMock.Object, ServiceProviderMock.Object, connectionFactoryMock.Object, ExchangeResolverMock.Object);
        }


        public void Dispose()
        {
            InitRabbitMQ();
        }
    }
}
