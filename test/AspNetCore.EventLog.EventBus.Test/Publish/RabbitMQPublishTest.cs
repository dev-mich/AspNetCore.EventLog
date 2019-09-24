using System;
using AspNetCore.EventLog.EventBus.Test.Fixtures;
using AspNetCore.EventLog.Interfaces;
using Moq;
using Xunit;

namespace AspNetCore.EventLog.EventBus.Test.Publish
{
    public class RabbitMQPublishTest: PublishTest, IClassFixture<RabbitMQFixture>
    {

        private readonly RabbitMQFixture fixture;


        public RabbitMQPublishTest(RabbitMQFixture fixture)
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

            var bus = InitEventBus();

            fixture.ExchangeResolverMock.Setup(e => e.ResolveExchange(It.IsAny<string>())).Returns((string)null);

            Assert.Throws<ArgumentNullException>(() => bus.Publish("event", "content"));

        }
    }
}
