using System;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.Logging;

namespace AspNetCore.EventLog.Infrastructure
{
    class ConsumerRegister
    {

        private readonly ILogger<ConsumerRegister> _logger;
        private readonly IEventBus _eventBus;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IReceivedStore _receivedStore;

        public ConsumerRegister(ILogger<ConsumerRegister> logger, IEventBus eventBus, IBackgroundTaskQueue taskQueue, IReceivedStore receivedStore)
        {
            _logger = logger;
            _eventBus = eventBus;
            _taskQueue = taskQueue;
            _receivedStore = receivedStore;
        }


        public void Register()
        {
            _eventBus.OnEventReceived += (sender, received) =>
            {
                _logger.LogInformation($"event received, begin persistence logic for event {received.Id}");

                try
                {
                    // check if event with same id already exist
                    var exist = _receivedStore.Find(received.Id);

                    if (exist != null)
                    {
                        _logger.LogInformation($"event with id {received.Id} already exist");
                        _eventBus.Commit();
                        return;
                    }

                    _receivedStore.Add(received);

                    _eventBus.Commit();

                    _logger.LogInformation($"event {received.Id} committed");

                    _taskQueue.QueueReceivedEvent(received);

                    _logger.LogInformation($"event {received.Id} enqueued");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"error during event {received.Id} persistence: {ex.Message}");

                    // something was wrong with event persist, reject
                    _eventBus.Reject();
                }

            };
        }

    }
}
