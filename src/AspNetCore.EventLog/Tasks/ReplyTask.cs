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

                _logger.LogInformation("start check for waiting replies");

                var waitingReplies = await receivedStore.GetAwaitingReplies();

                _logger.LogInformation($"fund {waitingReplies?.Count} wairing replies");

                foreach (var waiting in waitingReplies)
                {

                    try
                    {
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
                        _eventBus.Publish(waiting.ReplyTo, waiting.ReplyContent);


                        // update reply state
                        waiting.ReplyState = ReplyState.Forwarded;
                        waiting.ReplySended = DateTime.UtcNow;
                        await receivedStore.UpdateAsync(waiting);
                    } catch(Exception ex)
                    {
                        _logger.LogInformation($"reply publish failed due to: {ex.Message}");
                    }

                }

                _logger.LogInformation($"replies check completed");

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

            _logger.LogInformation("reply task is stopping");
        }
    }
}
