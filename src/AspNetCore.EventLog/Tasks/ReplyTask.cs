using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Tasks
{
    class ReplyTask : BackgroundService
    {
        private readonly ILogger<ReplyTask> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly CancellationTokenSource _shutdown;

        private Task _backgroundTask;


        public ReplyTask(ILogger<ReplyTask> logger, IServiceProvider serviceProvider, IEventBus eventBus, IBackgroundTaskQueue taskQueue)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _eventBus = eventBus;
            _backgroundTaskQueue = taskQueue;
            _shutdown = new CancellationTokenSource();
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("reply handler task starting");

            _backgroundTask = Task.Run(async () =>
            {
                await Processing();
            }, stoppingToken);

            await _backgroundTask;

            _logger.LogInformation("reply handler task stopping");

        }

        private async Task Processing()
        {
            while(!_shutdown.IsCancellationRequested)
            {
                _logger.LogInformation("waiting for pending replies");

                var receivedStore = _serviceProvider.GetRequiredService<IReceivedStore>();

                var waiting = await _backgroundTaskQueue.DequeueReplyAsync(_shutdown.Token);

                try
                {
                    _logger.LogInformation($"start sending reply for event {waiting.Id} with correlation id {waiting.CorrelationId} and routing key {waiting.ReplyTo}");

                    // try to update reply send date to ensure that job fail in concurrency situations
                    waiting.ReplySended = DateTime.UtcNow;

                    await receivedStore.UpdateAsync(waiting);

                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogError($"reply {waiting.Id} skipped due to concurrency");

                    continue;
                }


                try
                {
                    // now concurrency situations is avoided, publish reply
                    _eventBus.Publish(waiting.ReplyTo, waiting.ReplyContent, "", waiting.CorrelationId);


                    // update reply state
                    waiting.ReplyState = ReplyState.Forwarded;
                    waiting.ReplySended = DateTime.UtcNow;
                    await receivedStore.UpdateAsync(waiting);

                    _logger.LogInformation($"reply published for event {waiting.Id} with correlation id {waiting.CorrelationId} with routing key {waiting.ReplyTo}");

                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"reply publish failed due to: {ex.Message}");

                    _backgroundTaskQueue.QueueReplyEvent(waiting);
                }

            }
        }

    }
}
