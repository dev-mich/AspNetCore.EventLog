using System;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.EventBus.Test.Fixtures;
using AspNetCore.EventLog.EventBus.Test.Utils;
using AspNetCore.EventLog.RabbitMQ.Abstractions;
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
        public void TestUnresolvedExchange()
        {
            fixture.ExchangeResolverMock.Setup(x => x.ResolveExchange(It.IsAny<string>())).Returns((string)null);

            Assert.Throws<ArgumentNullException>(() => fixture.RabbitMq.Subscribe<IIntegrationEvent>("event"));
        }


        [Fact]
        public void TestUnresolvedQueue()
        {
            fixture.ExchangeResolverMock.Setup(x => x.ResolveExchange(It.IsAny<string>())).Returns("exchange");

            var queueResolverMock = new Mock<IQueueResolver>();

            queueResolverMock.Setup(s => s.ResolveQueue(It.IsAny<string>())).Returns((string) null);

            fixture.ServiceProviderMock.ResolveService<IQueueResolver>(queueResolverMock);

            Assert.Throws<ArgumentNullException>(() => fixture.RabbitMq.Subscribe<IIntegrationEvent>(It.IsAny<string>()));

        }

    }
}
