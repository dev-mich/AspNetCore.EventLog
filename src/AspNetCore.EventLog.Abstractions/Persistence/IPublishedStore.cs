using AspNetCore.EventLog.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Abstractions.Persistence
{
    public interface IPublishedStore : IStore<Published>
    {
        Task<List<Published>> GetPendingByPublisher(string publisher);

        Task SetEventState(Guid id, EventState state);

    }
}
