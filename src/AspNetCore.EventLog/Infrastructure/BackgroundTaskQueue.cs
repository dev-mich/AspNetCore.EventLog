using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Interfaces;

namespace AspNetCore.EventLog.Infrastructure
{
    class BackgroundTaskQueue: IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Received> _workItems = new ConcurrentQueue<Received>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueReceivedEvent(Received @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            _workItems.Enqueue(@event);
            _signal.Release();
        }

        public async Task<Received> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}
