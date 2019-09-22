using System;
using AspNetCore.EventLog.Abstractions.EventHandling;
using Xunit;

namespace AspNetCore.EventLog.EventBus.Test.Subscribe
{
    public abstract class SubscribeTest
    {

        protected abstract IEventBus InitEventBus();

        [Fact]
        public void TestMissingEventName()
        {

            var bus = InitEventBus();

            Assert.Throws<ArgumentNullException>(() => bus.Subscribe<IIntegrationEvent>(null));
            Assert.Throws<ArgumentNullException>(() => bus.Subscribe<IIntegrationEvent>(""));

        }


    }
}
