using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.EventLog.Tasks
{
    public class RetryPublishTask: BackgroundService
    {
        private readonly IPublishedStore _publishedStore;
        private readonly IEventBus _eventBus;

        public RetryPublishTask(IPublishedStore publishedStore, IEventBus eventBus)
        {
            _publishedStore = publishedStore;
            _eventBus = eventBus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var failed = await _publishedStore.GetFailed();

                foreach (var fail in failed)
                {
                    try
                    {
                        await _publishedStore.SetEventState(fail.Id, PublishedState.InProgress);

                        _eventBus.Publish(fail.EventName, fail.Content);

                        await _publishedStore.SetEventState(fail.Id, PublishedState.Published);
                    }
                    catch (Exception ex)
                    {
                        await _publishedStore.SetEventState(fail.Id, PublishedState.PublishedFailed);
                    }

                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
