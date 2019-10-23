using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.EventLog.Tasks
{
    class PublisherTask : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly ILogger<PublisherTask> _logger;
        private readonly CancellationTokenSource _shutdown;

        private Task _backgroundTask;

        public PublisherTask(IServiceProvider serviceprovider, IEventBus eventBus, IBackgroundTaskQueue backgroundTaskQueue, ILogger<PublisherTask> logger)
        {
            _serviceProvider = serviceprovider;
            _eventBus = eventBus;
            _backgroundTaskQueue = backgroundTaskQueue;
            _logger = logger;
            _shutdown = new CancellationTokenSource();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("publisher handler task starting");

            _backgroundTask = Task.Run(async () =>
            {
                await Processing();
            }, stoppingToken);

            await _backgroundTask;

            _logger.LogInformation("publisher handler task stopping");

        }


        private async Task Processing()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                _logger.LogInformation("waiting for pending event to event bus");

                var publishedStore = _serviceProvider.GetRequiredService<IPublishedStore>();

                var @event = await _backgroundTaskQueue.DequeuePublisheddAsync(_shutdown.Token);

                _logger.LogInformation($"found pending event {@event.Id}");

                try
                {
                    await publishedStore.SetEventStateAsync(@event.Id, PublishedState.InProgress);

                    _eventBus.Publish(@event.EventName, @event.Content, @event.ReplyTo, @event.CorrelationId);

                    await publishedStore.SetEventStateAsync(@event.Id, PublishedState.Published);

                    _logger.LogInformation($"pending event {@event.Id} published at {DateTime.UtcNow:dd/MM/yyyy : HH:mm}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"failed to publish event {@event.Id} due to: {ex.Message}");

                    await publishedStore.SetEventStateAsync(@event.Id, PublishedState.PublishedFailed);
                }
            }
        }
    }
}
