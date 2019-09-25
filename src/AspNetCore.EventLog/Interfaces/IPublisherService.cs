using System.Threading.Tasks;
using AspNetCore.EventLog.Infrastructure;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IPublisherService
    {
        Task Publish(string eventName, IIntegrationEvent @event);

        void SetTransaction(EventLogTransaction transaction);
    }
}
