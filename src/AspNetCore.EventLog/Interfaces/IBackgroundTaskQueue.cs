using System.Threading;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;

namespace AspNetCore.EventLog.Interfaces
{
    interface IBackgroundTaskQueue
    {
        void QueueReceivedEvent(Received @event);

        Task<Received> DequeueReceivedAsync(
            CancellationToken cancellationToken);

        void QueuePublishedEvent(Published @event);

        Task<Published> DequeuePublisheddAsync(
            CancellationToken cancellationToken);

        void QueueReplyEvent(Received @event);

        Task<Received> DequeueReplyAsync(
            CancellationToken cancellationToken);
    }
}
