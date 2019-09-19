using System;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.RabbitMQ.Config;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.EventLog.RabbitMQ.Extensions
{
    public static class DependencyInjection
    {

        public static void AddRabbitmq(this IServiceCollection services, Action<RabbitMqConfiguration> options)
        {

            services.Configure(options);

            services.AddSingleton<IEventBus, RabbitMqEventBus>();

        }

    }
}
