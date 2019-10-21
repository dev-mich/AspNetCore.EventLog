using System.Threading.Tasks;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IReplyHandler<in TReply> where TReply: IEventReply
    {

        Task<bool> Handle(TReply reply);

    }
}
