using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.EventLog.Tasks
{
    class RetryPublishTask: BackgroundService
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IPublishedStore _publishedStore;

        public RetryPublishTask(IBackgroundTaskQueue backgroundTaskQueue, IPublishedStore publishedStore)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _publishedStore = publishedStore;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var failed = await _publishedStore.GetFailed();

                foreach (var fail in failed)
                {
                    _backgroundTaskQueue.QueuePublishedEvent(fail);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
