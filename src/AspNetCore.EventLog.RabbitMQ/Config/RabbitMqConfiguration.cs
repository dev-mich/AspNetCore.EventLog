﻿
namespace AspNetCore.EventLog.RabbitMQ.Config
{
    public class RabbitMqConfiguration
    {

        public string HostName { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ExchangeName { get; set; }

        public string QueueName { get; set; }

    }
}
