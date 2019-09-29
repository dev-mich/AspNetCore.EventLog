using System;
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
    class PublisherService : IPublisherService
    {
        private readonly IPublishedStore _publishedStore;
        private readonly IEventBus _eventBus;
        private readonly EventLogOptions _options;
        private readonly ILogger<PublisherService> _logger;

        public PublisherService(IPublishedStore publishedStore, IEventBus eventBus, IOptions<EventLogOptions> options,
            ILogger<PublisherService> logger)
        {
            _publishedStore = publishedStore;
            _eventBus = eventBus;
            _logger = logger;
            _options = options.Value;
        }


        public Task Publish(string eventName, IIntegrationEvent @event)
        {
            var json = JsonConvert.SerializeObject(@event, _options.JsonSettings);
            var entry = Published.CreateEventLog(@event.Id, eventName, json);
            return _publishedStore.AddAsync(entry);
        }

        public void SetTransaction(EventLogTransaction transaction)
        {

            transaction.OnCommit += Transaction_OnCommit;
            _publishedStore.UseTransaction(transaction.DbTransaction);
        }

        private void Transaction_OnCommit()
        {
            _logger.LogInformation("start checking for pending events to publish");

            var pendings = _publishedStore.GetPending();

            _logger.LogInformation($"found {pendings.Count} events pending");

            foreach (var pending in pendings)
            {
                try
                {
                    _publishedStore.SetEventStateAsync(pending.Id, PublishedState.InProgress);

                    _eventBus.Publish(pending.EventName, pending.Content);

                    _publishedStore.SetEventStateAsync(pending.Id, PublishedState.Published);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Dispatch failed for event {pending.Id} of type {pending.EventName} due to {ex.Message}");

                    _publishedStore.SetEventStateAsync(pending.Id, PublishedState.PublishedFailed);
                }
            }

            _logger.LogInformation("pending events published");

        }
    }
}
