using System;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.Abstractions.Persistence;
using AspNetCore.EventLog.Core.Exceptions;
using AspNetCore.EventLog.Entities;
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

        public void Subscribe<TEvent>(string eventName) where TEvent : IIntegrationEvent
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

            var mqConsumer = new AsyncEventingBasicConsumer(_channel);

            mqConsumer.Received += Consume;


        }


        private async Task Consume(object sender, BasicDeliverEventArgs @event)
        {

            var processor = _serviceProvider.GetRequiredService<IMessageProcessor>();

            try
            {
                var content = Encoding.UTF8.GetString(@event.Body);

                var baseJsonContent = JsonConvert.DeserializeObject<IIntegrationEvent>(content);

                await processor.PersistEvent(baseJsonContent.Id, @event.RoutingKey, content);

                // event is persisted, ack the message
                _channel.BasicAck(@event.DeliveryTag, false);

                // consume
                await processor.Process(@event.RoutingKey, content);

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
            catch (ArgumentNullException)
            {
                // consumer not resolved, reject message
                _channel.BasicReject(@event.DeliveryTag, false);
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


        }



        private void Log(LogLevel level, string message)
        {
            var logger = _serviceProvider.GetService<ILogger<RabbitMqEventBus>>();

            logger?.Log(level, message);
        }


        public void Dispose()
        {
            _connection.Close();
            _channel.Close();
        }
    }
}
