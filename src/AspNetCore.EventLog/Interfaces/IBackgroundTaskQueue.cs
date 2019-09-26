using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;

namespace AspNetCore.EventLog.Interfaces
{
    interface IBackgroundTaskQueue
    {
        void QueueReceivedEvent(Received @event);

        Task<Received> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
