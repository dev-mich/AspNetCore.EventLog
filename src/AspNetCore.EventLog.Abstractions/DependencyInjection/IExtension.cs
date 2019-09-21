
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.Abstractions.DependencyInjection
{
    public interface IExtension
    {

        void AddServices(IServiceCollection services);

    }
}
