using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;

namespace AspNetCore.EventLog.Abstractions.Persistence
{
    public interface IReceivedStore : IStore<Received>
    {
        Task SetEventState(Guid id, ReceivedState state);

        Task<List<Received>> GetFailed();
    }
}
