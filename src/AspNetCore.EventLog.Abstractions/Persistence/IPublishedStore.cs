using AspNetCore.EventLog.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Abstractions.Persistence
{
    public interface IPublishedStore : IStore<Published>
    {
        Task<List<Published>> GetPendingByTransaction(Guid transactionId);

        Task SetEventState(Guid id, PublishedState state);

    }
}
