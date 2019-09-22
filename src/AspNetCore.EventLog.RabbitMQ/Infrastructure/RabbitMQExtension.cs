using System;
using AspNetCore.EventLog.Abstractions.DependencyInjection;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.RabbitMQ.Abstractions;
using AspNetCore.EventLog.RabbitMQ.Config;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace AspNetCore.EventLog.RabbitMQ.Infrastructure
{
    public class RabbitMQExtension : IExtension
    {
        private readonly RabbitMqConfiguration _configuration;

        public RabbitMQExtension(RabbitMqConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void AddServices(IServiceCollection services)
        {

            services.AddSingleton<IEventBus, RabbitMqEventBus>();
            services.AddSingleton<IConnectionFactory>(x => new ConnectionFactory
            {
                HostName = _configuration.HostName,
                Port = _configuration.Port,
                UserName = _configuration.Username,
                Password = _configuration.Password,
                DispatchConsumersAsync = true
            });

            services.AddTransient<IMessageProcessor, RabbitMQMessageProcessor>();


            // ADD RESOLVERS

            //exchange resolver is mandatory
            if (_configuration.ExchangeResolver == null)
                throw new ArgumentNullException(nameof(_configuration.ExchangeResolver));

            services.AddSingleton(typeof(IExchangeResolver), _configuration.ExchangeResolver);

            /* queue and consumer resolvers are not mandatory since this library could be used only for publishing event, 
             * queue and consumer resolving are needed only for subscribe an event */
            if (_configuration.QueueResolver != null)
                services.AddSingleton(typeof(IQueueResolver), _configuration.QueueResolver);

            if (_configuration.ConsumerResolver != null)
                services.AddSingleton(typeof(IConsumerResolver), _configuration.ConsumerResolver);
        }
    }
}
