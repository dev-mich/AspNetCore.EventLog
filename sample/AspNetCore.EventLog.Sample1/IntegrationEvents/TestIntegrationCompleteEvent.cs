using AspNetCore.EventLog.Interfaces;
using System;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Sample1.IntegrationEvents
{
    public class TestIntegrationCompleteEvent: IIntegrationEvent
    {

        public Guid Id { get; set; }

        public DateTime CompleteTime { get; set; }


        public TestIntegrationCompleteEvent()
        {
            Id = Guid.NewGuid();
            CompleteTime = DateTime.Now;
        }

    }


    public class TestIntegrationCompleteEventHandler : IEventHandler<TestIntegrationCompleteEvent>
    {
        public void Dispose()
        {
        }

        public Task<(bool, IIntegrationEvent)> Handle(TestIntegrationCompleteEvent @event, string correlationId)
        {
            Console.WriteLine($"event {correlationId} completed as {@event.CompleteTime:dd/mm/yyyy}");

            return Task.FromResult<(bool, IIntegrationEvent)>((true, null));
        }
    }
}
