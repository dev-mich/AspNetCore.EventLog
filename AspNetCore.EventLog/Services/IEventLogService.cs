using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Services
{
    public interface IEventLogService
    {

        Task SaveEventAsync(object @event, DbTransaction transaction, Guid transactionId);


        Task DispatchByTransactionId(DbConnection connection, Guid transactionId);

    }
}
