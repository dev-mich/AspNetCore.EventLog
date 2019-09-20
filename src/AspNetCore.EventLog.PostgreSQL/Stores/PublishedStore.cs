using System;
using System.Collections.Generic;
using System.Data.Common;
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

        public PublishedStore(PostgresDbContext context): base(context) { }


        public Task<List<Published>> GetPendingByPublisher(string publisher)
        {
            return DbSet.Where(e => e.PublisherName == publisher && e.EventState == EventState.NotPublished).ToListAsync();
        }

        public async Task SetEventState(Guid id, EventState state)
        {
            var @event = await DbSet.FindAsync(id);

            @event.EventState = state;

            await UpdateAsync(@event);
        }


    }
}
