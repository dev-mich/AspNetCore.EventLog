using System;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.EventBus.Test.Fixtures;
using AspNetCore.EventLog.RabbitMQ.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AspNetCore.EventLog.EventBus.Test.Subscribe
{
    public class RabbitMQSubscribeTest : SubscribeTest, IClassFixture<RabbitMQFixture>
    {
        private readonly RabbitMQFixture fixture;

        public RabbitMQSubscribeTest(RabbitMQFixture fixture)
        {
            this.fixture = fixture;
        }

        protected override IEventBus InitEventBus()
        {
            return fixture.RabbitMq;
        }


        [Fact]
        public void TestUnresolvedQueue()
        {

            var queueResolverMock = new Mock<IQueueResolver>();

            queueResolverMock.Setup(s => s.ResolveQueue(It.IsAny<string>())).Returns((string) null);

            fixture.ServiceProviderMock = new Mock<IServiceProvider>();
            fixture.ServiceProviderMock
                .Setup(x => x.GetService(typeof(IQueueResolver)))
                .Returns(queueResolverMock);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(fixture.ServiceProviderMock.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            fixture.ServiceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);


            Assert.Throws<ArgumentNullException>(() => fixture.RabbitMq.Subscribe<IIntegrationEvent>(It.IsAny<string>()));

        }

    }
}
