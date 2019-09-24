using System;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Exceptions;
using AspNetCore.EventLog.Interfaces;

namespace AspNetCore.EventLog.Infrastructure
{
    public abstract class MessageProcessor : IMessageProcessor
    {

        private readonly IReceivedStore _receivedStore;

        private Received _storedEvent;

        protected MessageProcessor(IReceivedStore receivedStore)
        {
            _receivedStore = receivedStore;
        }


        protected abstract Task<bool> Consume(string eventName, string content);


        public async Task PersistEvent(Guid eventId, string eventName, string content)
        {

            // handle idempotency
            _storedEvent = await _receivedStore.FindAsync(eventId);

            // event is already stored but not maybe not acked in rabbitmq, so do nothing
            if (_storedEvent != null)
                throw new ReceivedEventAlreadyPersistedException(eventId, eventName);

            _storedEvent = new Received(eventId, eventName, content);

            await PersistEvent();


        }

        public async Task<bool> Process(string eventName, string content)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException(nameof(content));

            try
            {
                // set event as processing
                await SetEventState(ReceivedState.InProgress);

                var success = await Consume(eventName, content);

                var targetState = success ? ReceivedState.Consumed : ReceivedState.Rejected;

                await SetEventState(targetState);

                return success;

            }
            catch (PersistenceException)
            {
                // something was wrong with the persistence, for the moment do nothing
                throw;
            }
            catch (Exception)
            {
                await SetEventState(ReceivedState.ConsumeFailed);

                throw;
            }
        }

        public Task<bool> Process(Received @event)
        {
            _storedEvent = @event;

            return Process(@event.EventName, @event.Content);
        }


        private Task PersistEvent()
        {

            try
            {
                return _receivedStore.AddAsync(_storedEvent);
            }
            catch (Exception)
            {
                throw new ReceivedEventNotPersistedException(_storedEvent.EventName);
            }

        }

        private Task SetEventState(ReceivedState state)
        {
            try
            {
                _storedEvent.EventState = state;

                if (state == ReceivedState.ConsumeFailed)
                    _storedEvent.RetryCount += 1;

                return _receivedStore.UpdateAsync(_storedEvent);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(ex.Message);
            }

        }


    }
}
