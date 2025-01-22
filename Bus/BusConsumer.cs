using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;

namespace Bus
{
    public class BusConsumer : IBusConsumer
    {
        public bool ChannelConstructed => _channel != null;

        private readonly ILogger<BusConsumer> _logger;
        private IConnection _connection;
        private IChannel _channel;

        private readonly string _queueName;

        public BusConsumer(EndpointNameHolder nameHolder,
            ILogger<BusConsumer> logger)
        {
            _queueName = nameHolder.Name;
            _logger = logger;
        }

        public async Task InitializeConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(queue: _queueName, exclusive: false);
        }

        public async Task ListenAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                _logger.LogInformation($"Получено сообщение: {content}");

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(_queueName, false, consumer);
        }

        public void DisposeAsync()
        {
            _channel.CloseAsync();
            _connection.CloseAsync();
        }
    }
}
