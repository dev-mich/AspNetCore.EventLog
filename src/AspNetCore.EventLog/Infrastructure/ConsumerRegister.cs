using System;
using AspNetCore.EventLog.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore.EventLog.Infrastructure
{
    class ConsumerRegister
    {

        private readonly ILogger<ConsumerRegister> _logger;
        private readonly IEventBus _eventBus;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceProvider _serviceProvider;

        public ConsumerRegister(ILogger<ConsumerRegister> logger, IEventBus eventBus, IBackgroundTaskQueue taskQueue, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _eventBus = eventBus;
            _taskQueue = taskQueue;
            _serviceProvider = serviceProvider;
        }


        public void Register()
        {
            _eventBus.OnEventReceived += (sender, received) =>
            {
                _logger.LogInformation($"event received, begin persistence logic for event {received.Id}");

                try
                {

                    var receivedStore = _serviceProvider.GetRequiredService<IReceivedStore>();

                    // check if event with same id already exist
                    var exist = receivedStore.Find(received.Id);

                    if (exist != null)
                    {
                        _logger.LogInformation($"event with id {received.Id} already exist");
                        _eventBus.Commit();
                        return;
                    }

                    receivedStore.Add(received);

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
