using System.Threading.Tasks;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IEventHandler<in TEvent> where TEvent : IIntegrationEvent
    {

        Task<bool> Handle(TEvent @event);

    }
}
