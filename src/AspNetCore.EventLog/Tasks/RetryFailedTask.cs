using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.Abstractions.Persistence;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.EventLog.Tasks
{
    class RetryFailedTask : BackgroundService
    {
        private readonly IReceivedStore _receivedStore;
        private readonly IMessageProcessor _messageProcessor;

        public RetryFailedTask(IReceivedStore receivedStore, IMessageProcessor messageProcessor)
        {
            _receivedStore = receivedStore;
            _messageProcessor = messageProcessor;
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
                        await _messageProcessor.Process(fail);
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
