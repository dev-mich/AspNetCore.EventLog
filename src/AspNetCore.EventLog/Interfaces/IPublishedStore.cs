﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IPublishedStore : IStore<Published>
    {
        Task<List<Published>> GetPendingByTransaction(Guid transactionId);

        Task SetEventState(Guid id, PublishedState state);

    }
}