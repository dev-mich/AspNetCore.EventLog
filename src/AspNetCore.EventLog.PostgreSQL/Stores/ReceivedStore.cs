using System;
using System.Threading.Tasks;
using AspNetCore.EventLog.Abstractions.Persistence;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.PostgreSQL.Configuration;
using AspNetCore.EventLog.PostgreSQL.Infrastructure;
using Microsoft.Extensions.Options;

namespace AspNetCore.EventLog.PostgreSQL.Stores
{
    internal class ReceivedStore: StoreBase<Received>, IReceivedStore
    {
        public ReceivedStore(IOptions<PostgreSqlOptions> options) : base(options)
        {
        }

        public async Task SetEventState(Guid id, ReceivedState state)
        {
            var record = await DbSet.FindAsync(id);

            record.EventState = state;

            await UpdateAsync(record);
        }
    }
}
