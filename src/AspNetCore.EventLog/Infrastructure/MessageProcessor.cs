using System;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Exceptions;
using AspNetCore.EventLog.Interfaces;

namespace AspNetCore.EventLog.Infrastructure
{
    class MessageProcessor: IMessageProcessor
    {

        private readonly IReceivedStore _receivedStore;
        private readonly IBackgroundTaskQueue _taskQueue;


        protected MessageProcessor(IReceivedStore receivedStore, IBackgroundTaskQueue taskQueue)
        {
            _receivedStore = receivedStore;
            _taskQueue = taskQueue;
        }



        public async Task Process(Guid eventId, string eventName, string content)
        {

            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException(nameof(content));


            // handle idempotency
            var storedEvent = await _receivedStore.FindAsync(eventId);

            // event is already stored but maybe not acknowledged by the message broker
            if (storedEvent != null)
                throw new ReceivedEventAlreadyPersistedException(eventId, eventName);

            storedEvent = new Received(eventId, eventName, content);

            await PersistEvent(storedEvent);

            // enqueue event to queue
            _taskQueue.QueueReceivedEvent(storedEvent);
        }


        private async Task PersistEvent(Received @event)
        {
            try
            {
                await _receivedStore.AddAsync(@event);
            }
            catch (Exception)
            {
                throw new ReceivedEventNotPersistedException(@event.EventName);
            }
        }


    }
}
