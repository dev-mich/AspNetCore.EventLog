using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IPublishedStore : IStore<Published>
    {
        List<Published> GetPending();

        Task SetEventState(Guid id, PublishedState state);

        Task<List<Published>> GetFailed();

    }
}
