using System;
using AspNetCore.EventLog.Services;
using Microsoft.AspNetCore.Builder;
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




        public static void UseEventLog(this IApplicationBuilder app)
        {

            //using (var scope = app.ApplicationServices.CreateScope())
            //{

            //    var storeOptions = scope.ServiceProvider.GetRequiredService<IOptions<EventLogStoreOptions>>();

            //    var context = new EventLogDbContext(storeOptions.Value.ContextFactory.Invoke(),)

            //}

        }


    }
}
