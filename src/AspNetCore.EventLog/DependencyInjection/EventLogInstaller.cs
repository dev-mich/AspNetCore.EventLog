using System;
using AspNetCore.EventLog.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.DependencyInjection
{
    public static class EventLogInstaller
    {

        public static void AddEventLogStore(this IServiceCollection services, Action<EventLogStoreOptions> options)
        {


            services.Configure(options);


        }




        public static void AddEventLogServices(this IServiceCollection services)
        {
            services.AddTransient<IEventLogService, EventLogService>();
        }


    }
}
