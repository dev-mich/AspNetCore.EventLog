using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Infrastructure;
using AspNetCore.EventLog.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AspNetCore.EventLog.Tasks
{
    class ReceivedHandlerTask : BackgroundService
    {
        private readonly CancellationTokenSource _shutdown;
        private Task _backgroundTask;
        private readonly ILogger<ReceivedHandlerTask> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly SubscriptionManager _subscriptionManager;
        private readonly IServiceProvider _serviceProvider;

        public ReceivedHandlerTask(IBackgroundTaskQueue taskQueue,
            ILogger<ReceivedHandlerTask> logger, IBackgroundTaskQueue taskQueue1,
            SubscriptionManager subscriptionManager, IServiceProvider serviceProvider)
        {
            _shutdown = new CancellationTokenSource();
            _taskQueue = taskQueue;
            _logger = logger;
            _taskQueue = taskQueue1;
            _subscriptionManager = subscriptionManager;
            _serviceProvider = serviceProvider;
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
                _logger.LogInformation("received handler task started");

                var received = await _taskQueue.DequeueReceivedAsync(_shutdown.Token);

                _logger.LogInformation($"received event {received.Id} dequeued");

                var receivedStore = _serviceProvider.GetRequiredService<IReceivedStore>();

                try
                {

                    using (var scope = _serviceProvider.CreateScope())
                    {

                        // set event as in progress
                        received.EventState = ReceivedState.InProgress;
                        await receivedStore.UpdateAsync(received);

                        // resolve subscription
                        var subscriptionType = _subscriptionManager.ResolveSubscription(received.EventName);

                        var handlerType = typeof(IEventHandler<>);

                        var genericType = handlerType.MakeGenericType(subscriptionType);

                        // resolve handler
                        var handler = scope.ServiceProvider.GetService(genericType);

                        if (handler == null)
                        {
                            throw new ArgumentNullException(nameof(handler));
                        }

                        var methodInfo = handler.GetType().GetMethod("Handle");

                        if (methodInfo == null)
                        {
                            throw new ArgumentException(nameof(handler));
                        }

                        // deserialize content to class
                        var @event = JsonConvert.DeserializeObject(received.Content, subscriptionType);

                        // call handler
                        var (success, reply) = await (Task<(bool, IIntegrationEvent)>) methodInfo.Invoke(handler, new []{ @event, received.CorrelationId });

                        var targetState = success ? ReceivedState.Consumed : ReceivedState.Rejected;


                        // update stored state
                        received.EventState = targetState;

                        if (reply != null)
                        {
                            received.ReplyContent = JsonConvert.SerializeObject(reply);
                        }

                        await receivedStore.UpdateAsync(received);


                    }


                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogInformation(
                        $"concurrency exception occurred at {DateTime.UtcNow} for event with id {received.Id}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);

                    await receivedStore.SetEventState(received.Id, ReceivedState.ConsumeFailed);
                }


            }

        }
    }
}
