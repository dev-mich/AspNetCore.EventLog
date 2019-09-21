using System;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.RabbitMQ.Abstractions;
using AspNetCore.EventLog.RabbitMQ.Config;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace AspNetCore.EventLog.RabbitMQ.Extensions
{
    public static class DependencyInjection
    {

        public static void AddRabbitmq(this IServiceCollection services, RabbitMqConfiguration options)
        {

            if (options == null)
                throw new ArgumentNullException(nameof(options));


            services.AddSingleton<IEventBus, RabbitMqEventBus>();
            services.AddSingleton<IConnectionFactory>(x => new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.Username,
                Password = options.Password,
                DispatchConsumersAsync = true
            });



            // ADD RESOLVERS

            //exchange resolver is mandatory
            if (options.ExchangeResolver == null) 
                throw new ArgumentNullException(nameof(options.ExchangeResolver));

            services.AddSingleton(typeof(IExchangeResolver), options.ExchangeResolver);

            /* queue and consumer resolvers are not mandatory since this library could be used only for publishing event, 
             * queue and consumer resolving are needed only for subscribe an event */
            if (options.QueueResolver != null)
                services.AddSingleton(typeof(IQueueResolver), options.QueueResolver);

            if (options.ConsumerResolver != null)
                services.AddSingleton(typeof(IConsumerResolver), options.ConsumerResolver);

        }

    }
}
