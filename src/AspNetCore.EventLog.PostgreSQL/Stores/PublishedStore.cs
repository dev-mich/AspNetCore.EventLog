using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Interfaces;
using AspNetCore.EventLog.PostgreSQL.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.EventLog.PostgreSQL.Stores
{
    class PublishedStore : StoreBase<Published>, IPublishedStore
    {


        public PublishedStore(DbContextFactory contextFactory) : base(contextFactory)
        {
        }


        public List<Published> GetPending()
        {
            return Context.ChangeTracker.Entries<Published>().Select(c => c.Entity).ToList();
        }

        public async Task SetEventState(Guid id, PublishedState state)
        {
            var @event = await DbSet.FindAsync(id);

            @event.EventState = state;

            await UpdateAsync(@event);
        }

        public Task<List<Published>> GetFailed()
        {
            return DbSet.Where(p => p.EventState == PublishedState.PublishedFailed).ToListAsync();
        }
    }
}
