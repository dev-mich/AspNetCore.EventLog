using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.EventLog.Abstractions.Persistence;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.PostgreSQL.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.EventLog.PostgreSQL.Stores
{
    internal class ReceivedStore: StoreBase<Received>, IReceivedStore
    {
        public ReceivedStore(DbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task SetEventState(Guid id, ReceivedState state)
        {
            var record = await DbSet.FindAsync(id);

            record.EventState = state;

            await UpdateAsync(record);
        }

        public Task<List<Received>> GetFailed()
        {
            return DbSet.Where(e => e.EventState == ReceivedState.ConsumeFailed && e.RetryCount < 10).ToListAsync();
        }
    }
}
