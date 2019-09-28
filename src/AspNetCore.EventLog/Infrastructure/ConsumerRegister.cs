using System;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.Infrastructure
{
    class ConsumerRegister
    {

        private readonly IEventBus _eventBus;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IReceivedStore _receivedStore;

        public ConsumerRegister(IEventBus eventBus, IBackgroundTaskQueue taskQueue, IReceivedStore receivedStore)
        {
            _eventBus = eventBus;
            _taskQueue = taskQueue;
            _receivedStore = receivedStore;
        }


        public void Register()
        {
            _eventBus.OnEventReceived += (sender, received) =>
            {

                try
                {
                    // check if event with same id already exist
                    var exist = _receivedStore.Find(received.Id);

                    if (exist != null)
                    {
                        _eventBus.Commit();
                        return;
                    }

                    _receivedStore.Add(received);

                    _eventBus.Commit();

                    _taskQueue.QueueReceivedEvent(received);
                }
                catch (Exception ex)
                {
                    // something was wrong with event persist, reject
                    _eventBus.Reject();
                }

            };
        }

    }
}
