using AspNetCore.EventLog.Infrastructure;
using AspNetCore.EventLog.Interfaces;

namespace AspNetCore.EventLog.Services
{
    class ReceiverService : IReceiverService
    {
        private readonly SubscriptionManager _subscriptionManager;
        private readonly IEventBus _eventBus;


        public ReceiverService(SubscriptionManager subscriptionManager, IEventBus eventBus)
        {
            _subscriptionManager = subscriptionManager;
            _eventBus = eventBus;
        }


        public void Subscribe<TEvent>(string eventName) where TEvent : IIntegrationEvent
        {
            _subscriptionManager.RegisterSubscription<TEvent>(eventName);
            _eventBus.Subscribe(eventName);
        }


        public void Unsubscribe(string eventName)
        {
            throw new System.NotImplementedException();
        }
    }
}
