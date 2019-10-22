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


        public ReplyTask(ILogger<ReplyTask> logger, IServiceProvider serviceProvider, IEventBus eventBus)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _eventBus = eventBus;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("reply task is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                var receivedStore = _serviceProvider.GetRequiredService<IReceivedStore>();

                var waitingReplies = await receivedStore.GetAwaitingReplies();

                foreach (var waiting in waitingReplies)
                {

                    try
                    {
                        _logger.LogInformation($"start sending reply for event {waiting.Id} with correlation id {waiting.CorrelationId}");

                        // try to update reply send date to ensure that job fail in concurrency situations
                        waiting.ReplySended = DateTime.UtcNow;

                        await receivedStore.UpdateAsync(waiting);

                    } catch(DbUpdateConcurrencyException)
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

                        _logger.LogInformation($"reply sent for event {waiting.Id} with correlation id {waiting.CorrelationId}");

                    } catch(Exception ex)
                    {
                        _logger.LogInformation($"reply publish failed due to: {ex.Message}");
                    }

                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

            _logger.LogInformation("reply task is stopping");
        }
    }
}
