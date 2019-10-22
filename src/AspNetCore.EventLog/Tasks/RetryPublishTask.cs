using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.EventLog.Tasks
{
    class RetryPublishTask: BackgroundService
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IServiceProvider _serviceprovider;

        public RetryPublishTask(IBackgroundTaskQueue backgroundTaskQueue, IServiceProvider serviceProvider)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _serviceprovider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var publishedStore = _serviceprovider.GetRequiredService<IPublishedStore>();

                var failed = await publishedStore.GetFailed();

                foreach (var fail in failed)
                {
                    _backgroundTaskQueue.QueuePublishedEvent(fail);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
