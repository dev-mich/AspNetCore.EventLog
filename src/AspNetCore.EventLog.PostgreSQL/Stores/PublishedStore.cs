using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.EventLog.Abstractions.Persistence;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.PostgreSQL.Configuration;
using AspNetCore.EventLog.PostgreSQL.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AspNetCore.EventLog.PostgreSQL.Stores
{
    class PublishedStore : StoreBase<Published>, IPublishedStore
    {
        public PublishedStore(IOptions<PostgreSqlOptions> options) : base(options)
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
