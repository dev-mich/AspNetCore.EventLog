using System;
using AspNetCore.EventLog.Configuration;
using AspNetCore.EventLog.RabbitMQ.Config;
using AspNetCore.EventLog.RabbitMQ.Infrastructure;
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
