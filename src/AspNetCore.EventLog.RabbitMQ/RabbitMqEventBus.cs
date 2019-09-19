using System;
using System.Text;
using AspNetCore.EventLog.Abstractions.EventHandling;
using AspNetCore.EventLog.RabbitMQ.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AspNetCore.EventLog.RabbitMQ
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {

        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly RabbitMqConfiguration _options;

        private IConnection _connection;
        private IModel _channel;
        private int _recoveryFailedCount;

        public RabbitMqEventBus(ILogger<RabbitMqEventBus> logger, IOptions<RabbitMqConfiguration> options, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _options = options.Value;

            InitRabbitMq();
        }


        public void Publish(string eventName, string content)
        {

            var body = Encoding.UTF8.GetBytes(content);

            _channel.BasicPublish(_options.ExchangeName, eventName, basicProperties: null, body: body);

        }

        public void Subscribe(string eventName)
        {
            _channel.QueueBind(_options.QueueName, _options.ExchangeName, eventName);
        }


        private void InitRabbitMq()
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
                DispatchConsumersAsync = true
            };

            // create connection
            _connection = factory.CreateConnection();

            _connection.ConnectionShutdown += (sender, args) =>
            {
                _logger.LogCritical("connection with rabbitmq shutdown");
            };

            _connection.RecoverySucceeded += (sender, args) =>
            {
                _logger.LogInformation("connection with rabbitmq recovered");
            };

            _connection.ConnectionRecoveryError += (sender, args) =>
            {
                _recoveryFailedCount += 1;

                _logger.LogError($"connection recovery error nr {_recoveryFailedCount} of 10");

            };

            // create channel
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic);

            if (!string.IsNullOrEmpty(_options.QueueName))
            {
                _channel.QueueDeclare(_options.QueueName, durable: true, exclusive: false, autoDelete: false);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                // consumer.Received += Consumer_Received;

                _channel.BasicConsume(_options.QueueName, false, consumer);

                _logger.LogInformation("rabbitmq subscriber start");
            }
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



        public void Dispose()
        {
            _connection.Close();
            _channel.Close();
        }
    }
}
