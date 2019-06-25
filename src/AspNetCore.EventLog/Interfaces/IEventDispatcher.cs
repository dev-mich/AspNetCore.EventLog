using System.Threading.Tasks;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IEventDispatcher
    {

        Task Dispatch(object @event);

    }
}
