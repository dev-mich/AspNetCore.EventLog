using System;
using System.Text;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.RabbitMQ.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

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

        public void Subscribe<TEvent>(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            var queueResolver = _serviceProvider.GetRequiredService<IQueueResolver>();

            // resolve queue name
            var queueName = queueResolver.ResolveQueue(eventName);

            if (string.IsNullOrEmpty(queueName))
                throw new ArgumentNullException(nameof(queueName));

            var queue = _channel.QueueDeclarePassive(queueName);

            // resolve exchange name
            var exchangeName = _exchangeResolver.ResolveExchange(eventName);

            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentNullException(nameof(exchangeName));

            _channel.QueueBind(queueName, exchangeName, eventName);
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

        //private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        //{
        //    var content = Encoding.UTF8.GetString(@event.Body);

        //    _logger.LogInformation($"message received {content} with routing key {@event.RoutingKey}");

        //    try
        //    {

        //        using (var scope = _serviceProvider.CreateScope())
        //        {
        //            // resolve command type
        //            var subsManager = scope.ServiceProvider.GetRequiredService<IEnumerable<IEventHandler>>();

        //            var handler = subsManager.FirstOrDefault(s => s.GetEventName().Equals(@event.RoutingKey));

        //            if (handler == null)
        //                throw new ArgumentNullException(nameof(handler));

        //            await handler.Handle(content);

        //            _channel.BasicAck(@event.DeliveryTag, false);
        //        }

        //    }
        //    catch (ValidationException ex)
        //    {
        //        var error = ex.Errors.FirstOrDefault();

        //        _logger.LogError($"message was invalid and will be rejected, error: {error?.ErrorMessage ?? "unknown"}");

        //        _channel.BasicReject(@event.DeliveryTag, false);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"message handler failed, requeue, error: {ex.Message}");

        //        _channel.BasicReject(@event.DeliveryTag, true);
        //    }


        //}




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
