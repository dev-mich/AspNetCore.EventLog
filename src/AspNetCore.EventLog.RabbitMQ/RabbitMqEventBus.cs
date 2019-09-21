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

            var queueResolver = _serviceProvider.GetRequiredService<IQueueResolver>();
            var consumerResolver = _serviceProvider.GetRequiredService<IConsumerResolver>();

            // resolve queue name
            var queueName = queueResolver.ResolveQueue(eventName);

            if (string.IsNullOrEmpty(queueName))
                throw new ArgumentNullException(nameof(queueName));

            _channel.QueueDeclarePassive(queueName);

            // resolve exchange name
            var exchangeName = _exchangeResolver.ResolveExchange(eventName);

            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentNullException(nameof(exchangeName));

            _channel.QueueBind(queueName, exchangeName, eventName);


            // resolve consumer
            var consumer = consumerResolver.ResolveConsumer(eventName);

            if (consumer == null)
                throw new ArgumentNullException(nameof(consumer));

            var mqConsumer = new AsyncEventingBasicConsumer(_channel);

            mqConsumer.Received += (sender, @event) => Consume(@event, consumer);


        }

        private async Task Consume(BasicDeliverEventArgs @event, Func<string, Task<bool>> consumer)
        {
            var store = _serviceProvider.GetRequiredService<IReceivedStore>();

            Received storedEvent = null;

            try
            {
                var content = Encoding.UTF8.GetString(@event.Body);

                var baseJsonContent = JsonConvert.DeserializeObject<IIntegrationEvent>(content);

                // try to get event by id
                storedEvent = new Received(baseJsonContent.Id, @event.RoutingKey, content);

                // persist received event
                if (!(await store.AddAsync(storedEvent)))
                    throw new ReceivedEventNotPersistedException(@event.RoutingKey);

                // call consumer
                var success = await consumer(content);

                if (success)
                {
                    _channel.BasicAck(@event.DeliveryTag, false);
                    storedEvent.EventState = ReceivedState.Consumed;
                }
                else
                {
                    storedEvent.EventState = ReceivedState.Rejected;
                    _channel.BasicReject(@event.DeliveryTag, false);
                }

                await store.UpdateAsync(storedEvent);

            }
            catch (ReceivedEventNotPersistedException)
            {
                _channel.BasicReject(@event.DeliveryTag, true);

                throw;
            }
            catch (Exception)
            {
                // consume failed but the event is stored in EventLog, so ack rabbitmq and wait for internal retry policy
                if (storedEvent != null)
                {
                    storedEvent.RetryCount = 1;
                    storedEvent.EventState = ReceivedState.ConsumeFailed;

                    await store.UpdateAsync(storedEvent);
                }

                _channel.BasicAck(@event.DeliveryTag, false);

                throw;
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
