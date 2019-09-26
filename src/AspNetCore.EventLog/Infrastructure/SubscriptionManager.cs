using AspNetCore.EventLog.Exceptions;
using AspNetCore.EventLog.Interfaces;
using System;
using System.Collections.Generic;

namespace AspNetCore.EventLog.Infrastructure
{
    internal class SubscriptionManager
    {

        private IDictionary<string, Type> _registeredSubscriptions;

        public SubscriptionManager()
        {
            _registeredSubscriptions = new Dictionary<string, Type>();
        }

        public void RegisterSubscription<TEvent>(string eventName) where TEvent : IIntegrationEvent
        {
            ValidateEventName(eventName);

            // ensure that a subscription for same event name dows not exist
            if (_registeredSubscriptions.TryGetValue(eventName, out Type subscription))
                return; // for the moment ignore the situation

            // register subscription for type
            _registeredSubscriptions.Add(eventName, typeof(TEvent));

        }


        public Type ResolveSubscription(string eventName)
        {
            ValidateEventName(eventName);

            if (!_registeredSubscriptions.TryGetValue(eventName, out Type result))
                throw new SubscriptionNotResolvedException(eventName);

            return result;
        }


        private void ValidateEventName(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));
        }

    }
}
