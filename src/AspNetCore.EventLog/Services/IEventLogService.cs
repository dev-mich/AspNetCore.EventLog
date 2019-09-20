using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Services
{
    public interface IEventLogService
    {

        Task SaveEventAsync(string publisherName, string eventName, object @event, DbTransaction transaction);


        Task SaveEventsAsync(string publisherName, IDictionary<string, object> events, DbTransaction transaction);


        Task DispatchByPublisher(DbConnection connection, string publisher);

    }
}
