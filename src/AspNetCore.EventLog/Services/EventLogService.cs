using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCore.EventLog.DependencyInjection;
using AspNetCore.EventLog.Exceptions;
using AspNetCore.EventLog.Infrastructure;
using AspNetCore.EventLog.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AspNetCore.EventLog.Services
{
    public class EventLogService: IEventLogService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly EventLogStoreOptions _options;
        private readonly ILogger<EventLogService> _logger;



        public EventLogService(IServiceProvider provider, IOptions<EventLogStoreOptions> options, ILogger<EventLogService> logger)
        {
            _serviceProvider = provider;
            _logger = logger;
            _options = options.Value;
        }


        protected EventLogDbContext Context;


        public Task SaveEventAsync(object @event, DbTransaction transaction, Guid transactionId)
        {
            if(transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            CreateConnection(transaction.Connection);

            var entry = new EventLog().CreateEventLog(@event, transactionId, _options.JsonSettings);

            Context.Database.UseTransaction(transaction);
            Context.EventLogs.Add(entry);

            return Context.SaveChangesAsync();

        }


        public Task SaveEventsAsync(IEnumerable<object> events, DbTransaction transaction, Guid transactionId)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            CreateConnection(transaction.Connection);

            var entries = events.Select(@event => new EventLog().CreateEventLog(@event, transactionId, _options.JsonSettings));


            Context.Database.UseTransaction(transaction);
            Context.EventLogs.AddRange(entries);

            return Context.SaveChangesAsync();
        }



        public async Task DispatchByTransactionId(DbConnection connection, Guid transactionId)
        {
            var dispatcher = _serviceProvider.GetService<IEventDispatcher>();

            if (dispatcher == null)
                throw new ArgumentNullException(nameof(dispatcher));

            CreateConnection(connection);


            // get all not published events by transactionId
            var pending = await GetPendingByTransactionId(transactionId);

            if (!pending.Any())
                return;

            foreach (var eventLog in pending)
            {
                try
                {
                    // set as in progress
                    await SetEventState(eventLog.Id, EventState.InProgress);

                    // get the event assembly
                    var assembly = Assembly.Load(eventLog.EventAssemblyName);

                    // get type by description
                    var type = assembly.GetType(eventLog.EventTypeName, true);

                    // deserialize to type
                    var @event = JsonConvert.DeserializeObject(eventLog.Content, type);

                    await dispatcher.Dispatch(@event);

                    await SetEventState(eventLog.Id, EventState.Published);

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Dispatch failed for event of type {eventLog.EventTypeName} due to {ex.Message}");

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



        private Task<List<EventLog>> GetPendingByTransactionId(Guid transactionId)
        {

            return Context.EventLogs.Where(e => e.TransactionId == transactionId && e.EventState == EventState.NotPublished).ToListAsync();

        }



        private async Task SetEventState(Guid id, EventState state)
        {

            var sql = new RawSqlString($"UPDATE {_options.DefaultSchema}.\"EventLogs\" SET \"EventState\" = {{0}} WHERE \"Id\" = {{1}}");

            await Context.Database.ExecuteSqlCommandAsync(sql, state, id);

        }


    }
}
