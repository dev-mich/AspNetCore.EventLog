using System;
using System.Threading.Tasks;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IMessageProcessor
    {
        Task Process(Guid eventId, string eventName, string content);
    }

}
