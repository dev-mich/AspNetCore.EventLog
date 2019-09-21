using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Services
{
    public interface IEventLogService
    {

        Task SaveEventAsync(Guid transactionId, string eventName, object @event, DbTransaction transaction);


        Task SaveEventsAsync(Guid transactionId, IDictionary<string, object> events, DbTransaction transaction);


        Task DispatchByPublisher(DbConnection connection, Guid transactionId);

    }
}
