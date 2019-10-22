using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.EventLog.Tasks
{
    class PublisherTask : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly CancellationTokenSource _shutdown;

        private Task _backgroundTask;

        public PublisherTask(IServiceProvider serviceprovider, IEventBus eventBus, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _serviceProvider = serviceprovider;
            _eventBus = eventBus;
            _backgroundTaskQueue = backgroundTaskQueue;
            _shutdown = new CancellationTokenSource();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _backgroundTask = Task.Run(async () =>
            {
                await Processing();
            }, stoppingToken);

            await _backgroundTask;

        }


        private async Task Processing()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                var publishedStore = _serviceProvider.GetRequiredService<IPublishedStore>();

                var @event = await _backgroundTaskQueue.DequeuePublisheddAsync(_shutdown.Token);

                try
                {
                    await publishedStore.SetEventStateAsync(@event.Id, PublishedState.InProgress);

                    _eventBus.Publish(@event.EventName, @event.Content, @event.ReplyTo, @event.CorrelationId);

                    await publishedStore.SetEventStateAsync(@event.Id, PublishedState.Published);
                }
                catch (Exception)
                {
                    await publishedStore.SetEventStateAsync(@event.Id, PublishedState.PublishedFailed);
                }
            }
        }
    }
}
