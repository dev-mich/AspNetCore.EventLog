using System;
using AspNetCore.EventLog.Entities;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IEventBus
    {

        void Publish(string eventName, string content);

        void Subscribe(string eventName);

        event EventHandler<Received> OnEventReceived;

        void Commit();

        void Reject();

    }
}
