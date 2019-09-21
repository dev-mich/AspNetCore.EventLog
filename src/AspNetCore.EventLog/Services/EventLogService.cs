﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.Abstractions.Persistence;
using AspNetCore.EventLog.Core.Configuration;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.EventLog.Services
{
    class EventLogService: IEventLogService
    {

        private readonly EventLogOptions _options;
        private readonly ILogger<EventLogService> _logger;
        private readonly IEventBus _eventBus;
        private readonly IPublishedStore _publishedStore;



        public EventLogService(IOptions<EventLogOptions> options, ILogger<EventLogService> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
            _options = options.Value;
        }



        public async Task SaveEventAsync(Guid transactionId, string eventName, object @event, DbTransaction transaction)
        {
            if(transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            var entry = Published.CreateEventLog(transactionId, eventName, @event, _options.JsonSettings);

            _publishedStore.UseTransaction(transaction);
            await _publishedStore.AddAsync(entry);

            return;

        }


        public async Task SaveEventsAsync(Guid transactionId, IDictionary<string, object> events, DbTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            var entries = events.Select(@event => Published.CreateEventLog(transactionId, @event.Key, @event.Value, _options.JsonSettings));


            _publishedStore.UseTransaction(transaction);
            await _publishedStore.AddAsync(entries);

        }



        public async Task DispatchByPublisher(DbConnection connection, Guid transactionId)
        {

            // get all not published events by publisher
            var pending = await _publishedStore.GetPendingByTransaction(transactionId);

            if (!pending.Any())
                return;

            foreach (var eventLog in pending)
            {
                try
                {
                    // set as in progress
                    await _publishedStore.SetEventState(eventLog.Id, PublishedState.InProgress);

                    _eventBus.Publish(eventLog.EventName, eventLog.Content);

                    await _publishedStore.SetEventState(eventLog.Id, PublishedState.Published);

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Dispatch failed for event {eventLog.Id} of type {eventLog.EventName} due to {ex.Message}");

                    await _publishedStore.SetEventState(eventLog.Id, PublishedState.PublishedFailed);

                    if (ex.GetType() == typeof(CriticalException))
                        throw;
                }
            }

        }



    }
}
