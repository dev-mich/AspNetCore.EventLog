
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.Abstractions.DependencyInjection
{
    public interface IExtensionBuilder
    {
        void AddServices(IServiceCollection services);

    }
}
