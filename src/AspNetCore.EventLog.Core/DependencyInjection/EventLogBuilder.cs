using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.Core.DependencyInjection
{
    public class EventLogBuilder
    {

        public IServiceCollection Services { get; }


        public EventLogBuilder(IServiceCollection services)
        {
            Services = services;
        }

    }
}
