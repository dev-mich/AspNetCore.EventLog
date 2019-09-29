using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.EventLog.Configuration;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Infrastructure;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AspNetCore.EventLog.Services
{
    class PublisherService : IPublisherService, IDisposable
    {
        private readonly IPublishedStore _publishedStore;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly EventLogOptions _options;
        private readonly ILogger<PublisherService> _logger;

        public PublisherService(IPublishedStore publishedStore, IBackgroundTaskQueue backgroundTaskQueue, IOptions<EventLogOptions> options,
            ILogger<PublisherService> logger)
        {
            _publishedStore = publishedStore;
            _backgroundTaskQueue = backgroundTaskQueue;
            _logger = logger;
            _options = options.Value;
        }


        private List<Published> _pendings;


        public async Task Publish(string eventName, IIntegrationEvent @event)
        {
            var json = JsonConvert.SerializeObject(@event, _options.JsonSettings);
            var entry = Published.CreateEventLog(@event.Id, eventName, json);
            await _publishedStore.AddAsync(entry);

            _pendings = _pendings ?? new List<Published>();
            _pendings.Add(entry);
        }

        public void SetTransaction(EventLogTransaction transaction)
        {

            transaction.OnCommit += Transaction_OnCommit;
            _publishedStore.UseTransaction(transaction.DbTransaction);
        }

        private void Transaction_OnCommit()
        {

            _logger.LogInformation($"found {_pendings.Count} events pending");

            foreach (var pending in _pendings)
            {
                _backgroundTaskQueue.QueuePublishedEvent(pending);
            }

            _logger.LogInformation("pending events published");

        }

        public void Dispose()
        {
            _pendings = null;
        }
    }
}
