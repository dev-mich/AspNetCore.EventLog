using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.PostgreSQL.Infrastructure;

namespace AspNetCore.EventLog.PostgreSQL.Stores
{
    internal class ReceivedStore: StoreBase<Received>
    {
        public ReceivedStore(PostgresDbContext context) : base(context)
        {
        }
    }
}
