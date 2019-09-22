using System;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.Core.Configuration;
using AspNetCore.EventLog.RabbitMQ.Abstractions;
using AspNetCore.EventLog.RabbitMQ.Config;
using AspNetCore.EventLog.RabbitMQ.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace AspNetCore.EventLog.RabbitMQ.Extensions
{
    public static class DependencyInjection
    {

        public static void AddRabbitmq(this EventLogOptions eventLogOptions, RabbitMqConfiguration options)
        {

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            eventLogOptions.RegisterExtension(new RabbitMQExtension(options));

        }

    }
}
