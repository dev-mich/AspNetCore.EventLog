using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.Sample1.IntegrationEvents;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Sample1.Tasks
{
    public class TestSubscribeTask : BackgroundService
    {
        private readonly IEventBus _eventBus;

        public TestSubscribeTask(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _eventBus.Subscribe<TestIntegrationEvent>("test.event");
            _eventBus.Subscribe<TestIntegrationEvent>("test.event.failed");

            return Task.CompletedTask;
        }
    }
}
