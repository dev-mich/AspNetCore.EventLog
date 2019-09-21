using System;
using AspNetCore.EventLog.Abstractions.EventHandling;
using Xunit;

namespace AspNetCore.EventLog.EventBus.Test.Publish
{
    public abstract class PublishTest
    {

        protected abstract IEventBus InitEventBus();


        [Fact]
        public void TestMissingEventName()
        {

            var bus = InitEventBus();

            Assert.Throws<ArgumentNullException>(() => bus.Publish(null, "aa"));
            Assert.Throws<ArgumentNullException>(() => bus.Publish("", "aa"));

        }


        [Fact]
        public void TestMissingContent()
        {

            var bus = InitEventBus();

            Assert.Throws<ArgumentNullException>(() => bus.Publish("event", null));
            Assert.Throws<ArgumentNullException>(() => bus.Publish("event", ""));

        }

    }
}
