using System;
using AspNetCore.EventLog.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.DependencyInjection
{
    public static class EventLogInstaller
    {

        public static void AddEventLog(this IServiceCollection services, Action<EventLogStoreOptions> options)
        {


            services.Configure(options);

            services.AddTransient<IEventLogService, EventLogService>();


        }



    }
}
