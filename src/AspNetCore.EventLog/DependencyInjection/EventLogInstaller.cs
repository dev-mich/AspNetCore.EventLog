using System;
using AspNetCore.EventLog.Configuration;
using AspNetCore.EventLog.Infrastructure;
using AspNetCore.EventLog.Interfaces;
using AspNetCore.EventLog.Services;
using AspNetCore.EventLog.Tasks;
using Microsoft.AspNetCore.Builder;
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

            services.AddTransient<IPublisherService, PublisherService>();
            services.AddTransient<IReceiverService, ReceiverService>();

            services.AddSingleton<ConsumerRegister>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<SubscriptionManager>();

            services.AddHostedService<MigratorTask>();
            services.AddHostedService<RetryPublishTask>();
            services.AddHostedService<RetryHandlerTask>();
            services.AddHostedService<ReceivedHandlerTask>();

            var options = new EventLogOptions();
            setupOptions(options);

            foreach (var optionsExtension in options.Extensions)
            {
                optionsExtension.AddServices(services);
            }

        }


        public static void UseEventLog(this IApplicationBuilder app)
        {
            // start consumer service instance
            var consumerRegister = app.ApplicationServices.GetRequiredService<ConsumerRegister>();

            consumerRegister.Register();
        }



    }
}
