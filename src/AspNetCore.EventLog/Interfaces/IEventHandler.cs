using System;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IEventHandler<in TEvent> : IDisposable where TEvent : IIntegrationEvent
    {

        Task<(bool, IIntegrationEvent)> Handle(TEvent @event, string correlationId);

    }
}
