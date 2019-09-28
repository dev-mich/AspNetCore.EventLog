using System;
using System.Threading.Tasks;
using AspNetCore.EventLog.Interfaces;

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

    public class TestIntegrationEventHandler : IEventHandler<TestIntegrationEvent>
    {
        public Task<bool> Handle(TestIntegrationEvent @event)
        {
            Console.WriteLine(@event.EventStuff);

            return Task.FromResult(true);
        }

        public void Dispose()
        {
        }
    }
}
