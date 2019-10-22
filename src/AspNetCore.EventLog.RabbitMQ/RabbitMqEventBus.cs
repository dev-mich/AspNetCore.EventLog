using System;
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

        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IExchangeResolver _exchangeResolver;

        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer mqConsumer;
        private int _recoveryFailedCount;
        private ulong _deliveryTag;

        public RabbitMqEventBus(ILogger<RabbitMqEventBus>  logger, IServiceProvider serviceProvider, IConnectionFactory rabbitMqConnectionFactory, IExchangeResolver exchangeResolver)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _connectionFactory = rabbitMqConnectionFactory;
            _exchangeResolver = exchangeResolver;
            _recoveryFailedCount = 0;

            InitRabbitMq();
        }


        public void Publish(string eventName, string content, string replyTo = null, string correlationId = null)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException(nameof(content));

            var body = Encoding.UTF8.GetBytes(content);

            // resolve exchange name
            var exchangeName = _exchangeResolver.ResolveExchange(eventName);

            if (exchangeName == null)
                throw new ArgumentNullException(nameof(exchangeName));

            // create basic properties
            var props = _channel.CreateBasicProperties();

            if (!string.IsNullOrEmpty(replyTo))
            {
                props.ReplyTo = replyTo;
                _channel.QueueDeclarePassive(replyTo);

                if (string.IsNullOrEmpty(correlationId))
                {
                    throw new ArgumentNullException(nameof(correlationId));
                }

            }

            props.CorrelationId = correlationId;

            // passive declare exchange to ensure that exist (only if not the default one)
            if (exchangeName != string.Empty)
            {
                _channel.ExchangeDeclarePassive(exchangeName);
            }

            _channel.BasicPublish(exchangeName, eventName, props, body);

        }

        public void Subscribe(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));


            // resolve exchange name
            var exchangeName = _exchangeResolver.ResolveExchange(eventName);

            if (exchangeName == null)
                throw new ArgumentNullException(nameof(exchangeName));

            var queueResolver = _serviceProvider.GetRequiredService<IQueueResolver>();

            // resolve queue name
            var queueName = queueResolver.ResolveQueue(eventName);

            if (string.IsNullOrEmpty(queueName))
                throw new ArgumentNullException(nameof(queueName));

            _channel.QueueDeclarePassive(queueName);

            // bind queue only if not working on default exchange
            if (exchangeName != string.Empty)
                _channel.QueueBind(queueName, exchangeName, eventName);

            _channel.BasicConsume(queueName, false, mqConsumer);

        }

        public event EventHandler<Received> OnEventReceived;

        public void Commit()
        {
            _channel.BasicAck(_deliveryTag, false);
        }

        public void Reject()
        {
            _channel.BasicReject(_deliveryTag, true);
        }


        private void Consume(object sender, BasicDeliverEventArgs @event)
        {
            _logger.LogInformation($"received message at {DateTime.UtcNow}, routing key: {@event.RoutingKey}");

            _deliveryTag = @event.DeliveryTag;

            var content = Encoding.UTF8.GetString(@event.Body);

            _logger.LogInformation($"raw content is {content}");

            var baseJsonContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

            var id = GetId(baseJsonContent);

            if (!id.HasValue)
                throw new ArgumentNullException(nameof(id));

            var received = new Received(id.Value, @event.RoutingKey, content, @event.BasicProperties.ReplyTo, @event.BasicProperties.CorrelationId);


            OnEventReceived?.Invoke(sender, received);


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


            mqConsumer = new EventingBasicConsumer(_channel);

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

            // search for lowercase also if not found for uppercase
            if (@event.TryGetValue("id", out id))
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
