﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.EventLog.Entities;
using AspNetCore.EventLog.Exceptions;
using AspNetCore.EventLog.Interfaces;
using AspNetCore.EventLog.RabbitMQ.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AspNetCore.EventLog.RabbitMQ
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IExchangeResolver _exchangeResolver;

        private IConnection _connection;
        private IModel _channel;
        private AsyncEventingBasicConsumer mqConsumer;
        private int _recoveryFailedCount;

        public RabbitMqEventBus(IServiceProvider serviceProvider, IConnectionFactory rabbitMqConnectionFactory, IExchangeResolver exchangeResolver)
        {
            _serviceProvider = serviceProvider;
            _connectionFactory = rabbitMqConnectionFactory;
            _exchangeResolver = exchangeResolver;
            _recoveryFailedCount = 0;

            InitRabbitMq();
        }


        public void Publish(string eventName, string content)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException(nameof(content));

            var body = Encoding.UTF8.GetBytes(content);

            // resolve exchange name
            var exchangeName = _exchangeResolver.ResolveExchange(eventName);

            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentNullException(nameof(exchangeName));

            // passive declare exchange to ensure that exist
            _channel.ExchangeDeclarePassive(exchangeName);

            _channel.BasicPublish(exchangeName, eventName, basicProperties: null, body: body);

        }

        public void Subscribe(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));


            // resolve exchange name
            var exchangeName = _exchangeResolver.ResolveExchange(eventName);

            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentNullException(nameof(exchangeName));

            var queueResolver = _serviceProvider.GetRequiredService<IQueueResolver>();

            // resolve queue name
            var queueName = queueResolver.ResolveQueue(eventName);

            if (string.IsNullOrEmpty(queueName))
                throw new ArgumentNullException(nameof(queueName));

            _channel.QueueDeclarePassive(queueName);

            _channel.QueueBind(queueName, exchangeName, eventName);

            _channel.BasicConsume(queueName, false, mqConsumer);

        }


        private async Task Consume(object sender, BasicDeliverEventArgs @event)
        {

            var processor = _serviceProvider.GetRequiredService<IMessageProcessor>();

            try
            {
                var content = Encoding.UTF8.GetString(@event.Body);

                var baseJsonContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                var id = GetId(baseJsonContent);

                if (!id.HasValue)
                    throw new ArgumentNullException(nameof(id));

                await processor.Process(id.Value, @event.RoutingKey, content);

                // event is persisted, ack the message
                _channel.BasicAck(@event.DeliveryTag, false);

            }
            catch (ReceivedEventAlreadyPersistedException)
            {
                // event is already stored, ack message
                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (ReceivedEventNotPersistedException)
            {
                // reject event and ask for requeue
                _channel.BasicReject(@event.DeliveryTag, true);
            }


        }


        private void InitRabbitMq()
        {


            // create connection
            _connection = _connectionFactory.CreateConnection();

            _connection.ConnectionShutdown += (sender, args) =>
            {
                Log(LogLevel.Critical, "connection with rabbitmq shutdown");
            };

            _connection.RecoverySucceeded += (sender, args) =>
            {
                _recoveryFailedCount = 0;
                Log(LogLevel.Information, "connection with rabbitmq recovered");
            };

            _connection.ConnectionRecoveryError += (sender, args) =>
            {
                _recoveryFailedCount += 1;

                Log(LogLevel.Error, $"connection recovery error nr {_recoveryFailedCount} of 10");

            };

            _channel = _connection.CreateModel();


            mqConsumer = new AsyncEventingBasicConsumer(_channel);

            mqConsumer.Received += Consume;

        }



        private void Log(LogLevel level, string message)
        {
            var logger = _serviceProvider.GetService<ILogger<RabbitMqEventBus>>();

            logger?.Log(level, message);
        }


        private Guid? GetId(Dictionary<string, object> @event)
        {
            if (@event.TryGetValue("Id", out object id))
                return Guid.Parse(id.ToString());

            return null;
        }


        public void Dispose()
        {
            _connection.Close();
            _channel.Close();
        }
    }
}
