using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.EventLog.Abstractions.Event;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.DependencyInjection;
using AspNetCore.EventLog.Exceptions;
using AspNetCore.EventLog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.EventLog.Services
{
    class EventLogService: IEventLogService
    {

        private readonly EventLogStoreOptions _options;
        private readonly ILogger<EventLogService> _logger;
        private readonly IEventBus _eventBus;



        public EventLogService(IOptions<EventLogStoreOptions> options, ILogger<EventLogService> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
            _options = options.Value;
        }


        protected EventLogDbContext Context;


        public Task SaveEventAsync(IntegrationEvent @event, DbTransaction transaction, Guid transactionId)
        {
            if(transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            CreateConnection(transaction.Connection);

            var entry = EventLog.CreateEventLog(@event, transactionId, _options.JsonSettings);

            Context.Database.UseTransaction(transaction);
            Context.Published.Add(entry);

            return Context.SaveChangesAsync();

        }


        public Task SaveEventsAsync(IEnumerable<IntegrationEvent> events, DbTransaction transaction, Guid transactionId)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            CreateConnection(transaction.Connection);

            var entries = events.Select(@event => EventLog.CreateEventLog(@event, transactionId, _options.JsonSettings));


            Context.Database.UseTransaction(transaction);
            Context.Published.AddRange(entries);

            return Context.SaveChangesAsync();
        }



        public async Task DispatchByPublisher(DbConnection connection, string publisher)
        {

            CreateConnection(connection);


            // get all not published events by transactionId
            var pending = await GetPendingByPublisher(publisher);

            if (!pending.Any())
                return;

            foreach (var eventLog in pending)
            {
                try
                {
                    // set as in progress
                    await SetEventState(eventLog.Id, EventState.InProgress);

                    _eventBus.Publish(eventLog.EventName, eventLog.Content);

                    await SetEventState(eventLog.Id, EventState.Published);

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Dispatch failed for event {eventLog.Id} of type {eventLog.EventName} due to {ex.Message}");

                    await SetEventState(eventLog.Id, EventState.PublishedFailed);

                    if (ex.GetType() == typeof(CriticalException))
                        throw;
                }
            }

        }




        private void CreateConnection(DbConnection connection)
        {
            var optBulder = _options.ContextFactory.Invoke(connection);

            Context = new EventLogDbContext(optBulder.Options, new OptionsWrapper<EventLogStoreOptions>(_options));
        }



        private Task<List<EventLog>> GetPendingByPublisher(string publisher)
        {

            return Context.Published.Where(e => e.PublisherName == publisher && e.EventState == EventState.NotPublished).ToListAsync();

        }



        private async Task SetEventState(Guid id, EventState state)
        {

            var sql = new RawSqlString($"UPDATE {_options.DefaultSchema}.\"EventLogs\" SET \"EventState\" = {{0}} WHERE \"Id\" = {{1}}");

            await Context.Database.ExecuteSqlCommandAsync(sql, state, id);

        }


    }
}
