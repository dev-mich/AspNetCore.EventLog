using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Infrastructure;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.EventLog.Tasks
{
    class RetryHandlerTask : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBackgroundTaskQueue _taskQueue;

        public RetryHandlerTask(IServiceProvider serviceProvider, IBackgroundTaskQueue taskQueue)//, MessageProcessor messageProcessor)
        {
            _serviceProvider = serviceProvider;
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var receivedStore = _serviceProvider.GetRequiredService<IReceivedStore>();

                var failed = await receivedStore.GetFailed();

                foreach (var fail in failed)
                {
                    try
                    {
                        // set event state as received
                        await receivedStore.SetEventState(fail.Id, ReceivedState.Received);

                        _taskQueue.QueueReceivedEvent(fail);
                    }
                    catch (Exception)
                    {
                        // do nothing
                    }

                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            }
        }
    }
}
