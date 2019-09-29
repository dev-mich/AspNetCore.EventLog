using AspNetCore.EventLog.Infrastructure;
using AspNetCore.EventLog.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace AspNetCore.EventLog.PostgreSQL.Extensions
{
    public static class DatabaseFacadeExtensions
    {

        public static EventLogTransaction BeginTransaction(this DatabaseFacade database, IPublisherService service)
        {

            var trans = database.BeginTransaction();

            var result = new EventLogTransaction(trans.GetDbTransaction());

            service.SetTransaction(result);

            return result;
        }

    }
}
