using AspNetCore.EventLog.Infrastructure;
using AspNetCore.EventLog.Interfaces;

namespace AspNetCore.EventLog.Services
{
    class ReceiverService : IReceiverService
    {
        private readonly SubscriptionManager _subscriptionManager;


        public ReceiverService(SubscriptionManager subscriptionManager)
        {
            _subscriptionManager = subscriptionManager;
        }


        public void Subscribe<TEvent>(string eventName) where TEvent : IIntegrationEvent
        {
            _subscriptionManager.RegisterSubscription<TEvent>(eventName);
        }


        public void Unsubscribe(string eventName)
        {
            throw new System.NotImplementedException();
        }
    }
}
