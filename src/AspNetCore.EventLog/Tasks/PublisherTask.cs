using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.EventLog.Tasks
{
    class PublisherTask : BackgroundService
    {
        private readonly IPublishedStore _publishedStore;
        private readonly IEventBus _eventBus;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly CancellationTokenSource _shutdown;

        private Task _backgroundTask;

        public PublisherTask(IPublishedStore publishedStore, IEventBus eventBus, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _publishedStore = publishedStore;
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
                var @event = await _backgroundTaskQueue.DequeuePublisheddAsync(_shutdown.Token);

                try
                {
                    await _publishedStore.SetEventStateAsync(@event.Id, PublishedState.InProgress);

                    _eventBus.Publish(@event.EventName, @event.Content, @event.ReplyTo, @event.CorrelationId);

                    await _publishedStore.SetEventStateAsync(@event.Id, PublishedState.Published);
                }
                catch (Exception ex)
                {
                    await _publishedStore.SetEventStateAsync(@event.Id, PublishedState.PublishedFailed);
                }
            }
        }
    }
}
