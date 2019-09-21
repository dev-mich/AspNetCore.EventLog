﻿using System;
using AspNetCore.EventLog.Core.Configuration;
using AspNetCore.EventLog.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.DependencyInjection
{
    public static class EventLogInstaller
    {

        public static void AddEventLog(this IServiceCollection services, Action<EventLogOptions> setupOptions)
        {
            if (setupOptions == null)
                throw new ArgumentNullException(nameof(setupOptions));

            services.Configure(setupOptions);

            services.AddTransient<IEventLogService, EventLogService>();

            var options = new EventLogOptions();
            setupOptions(options);

            foreach (var optionsExtension in options.Extensions)
            {
                optionsExtension.AddServices(services);
            }

        }



    }
}
