using AspNetCore.EventLog.Abstractions.EventHandling;
using System;

namespace AspNetCore.EventLog.Sample1.IntegrationEvents
{
    public class TestIntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; set; }

        public string EventStuff { get; set; }


        public TestIntegrationEvent()
        {
            Id = Guid.NewGuid();
            EventStuff = "test event property bla bla bla";
        }

    }
}
