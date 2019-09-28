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
        private readonly IReceivedStore _receivedStore;
        private readonly SubscriptionManager _subscriptionManager;
        private readonly IServiceProvider _serviceProvider;

        public ReceivedHandlerTask(IBackgroundTaskQueue taskQueue,
            ILogger<ReceivedHandlerTask> logger, IBackgroundTaskQueue taskQueue1, IReceivedStore receivedStore,
            SubscriptionManager subscriptionManager, IServiceProvider serviceProvider)
        {
            _shutdown = new CancellationTokenSource();
            _taskQueue = taskQueue;
            _logger = logger;
            _taskQueue = taskQueue1;
            _receivedStore = receivedStore;
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
                var received = await _taskQueue.DequeueAsync(_shutdown.Token);

                try
                {

                    using (var scope = _serviceProvider.CreateScope())
                    {

                        // set event as in progress
                        await _receivedStore.SetEventState(received.Id, ReceivedState.InProgress);

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
                        var success = await (Task<bool>) methodInfo.Invoke(handler, new []{ @event });

                        var targetState = success ? ReceivedState.Consumed : ReceivedState.Rejected;

                        await _receivedStore.SetEventState(received.Id, targetState);
                    }


                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogInformation(
                        $"concurrency exception occurred at {DateTime.UtcNow} for event with id {received.Id}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);

                    await _receivedStore.SetEventState(received.Id, ReceivedState.ConsumeFailed);
                }


            }

        }
    }
}
