
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.Interfaces
{
    public interface IExtension
    {

        void AddServices(IServiceCollection services);

    }
}
