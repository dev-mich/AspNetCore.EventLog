using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using AspNetCore.EventLog.Abstractions.Event;

namespace AspNetCore.EventLog.Services
{
    public interface IEventLogService
    {

        Task SaveEventAsync(IntegrationEvent @event, DbTransaction transaction, Guid transactionId);


        Task SaveEventsAsync(IEnumerable<IntegrationEvent> events, DbTransaction transaction, Guid transactionId);


        Task DispatchByPublisher(DbConnection connection, string publisher);

    }
}
