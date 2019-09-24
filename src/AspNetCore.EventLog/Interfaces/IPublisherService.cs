using System.Threading.Tasks;

namespace AspNetCore.EventLog.Interfaces
{
    interface IPublisherService
    {
        Task Publish(string eventName, object @event);
    }
}
