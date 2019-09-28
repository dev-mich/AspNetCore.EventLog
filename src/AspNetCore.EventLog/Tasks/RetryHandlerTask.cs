using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Infrastructure;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.EventLog.Tasks
{
    class RetryHandlerTask : BackgroundService
    {
        private readonly IReceivedStore _receivedStore;
        private readonly IBackgroundTaskQueue _taskQueue;

        public RetryHandlerTask(IReceivedStore receivedStore, IBackgroundTaskQueue taskQueue)//, MessageProcessor messageProcessor)
        {
            _receivedStore = receivedStore;
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                var failed = await _receivedStore.GetFailed();

                foreach (var fail in failed)
                {
                    try
                    {
                        // set event state as received
                        await _receivedStore.SetEventState(fail.Id, ReceivedState.Received);

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
