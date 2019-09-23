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
    class PublishedStore : StoreBase<Published>, IPublishedStore
    {


        public PublishedStore(DbContextFactory contextFactory) : base(contextFactory)
        {
        }


        public Task<List<Published>> GetPendingByTransaction(Guid transactionId)
        {
            return DbSet.Where(e => e.TransactionId == transactionId && e.EventState == PublishedState.NotPublished).ToListAsync();
        }

        public async Task SetEventState(Guid id, PublishedState state)
        {
            var @event = await DbSet.FindAsync(id);

            @event.EventState = state;

            await UpdateAsync(@event);
        }

    }
}
