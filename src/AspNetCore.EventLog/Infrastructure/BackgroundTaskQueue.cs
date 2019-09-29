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
        private readonly ConcurrentQueue<Received> _receivedItems = new ConcurrentQueue<Received>();
        private readonly SemaphoreSlim _receivedSignal = new SemaphoreSlim(0);

        private readonly ConcurrentQueue<Published> _publishedItems = new ConcurrentQueue<Published>();
        private readonly SemaphoreSlim _publishedSignal = new SemaphoreSlim(0);

        public void QueueReceivedEvent(Received @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            _receivedItems.Enqueue(@event);
            _receivedSignal.Release();
        }

        public async Task<Received> DequeueReceivedAsync(CancellationToken cancellationToken)
        {
            await _receivedSignal.WaitAsync(cancellationToken);
            _receivedItems.TryDequeue(out var workItem);

            return workItem;
        }

        public void QueuePublishedEvent(Published @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            _publishedItems.Enqueue(@event);
            _publishedSignal.Release();
        }

        public async Task<Published> DequeuePublisheddAsync(CancellationToken cancellationToken)
        {
            await _publishedSignal.WaitAsync(cancellationToken);
            _publishedItems.TryDequeue(out var workItem);

            return workItem;
        }

    }
}
