using Bus.Shared;
using DAO;
using DAO.Outbox;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace Bus
{
    public class BusConsumer : IBusConsumer
    {
        public bool ChannelConstructed => _channel != null;

        private readonly ILogger<BusConsumer> _logger;
        private readonly OutboxDao _outboxDao;
        private readonly IBusService _busService;
        private IConnection _connection;
        private IChannel _channel;

        private readonly string _queueName;

        public BusConsumer(EndpointNameHolder nameHolder,
            ILogger<BusConsumer> logger,
            OutboxDao outboxDao,
            IBusService busService)
        {
            _queueName = nameHolder.Name;
            _logger = logger;
            _outboxDao = outboxDao;
            _busService = busService;
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

                var busMessage = JsonSerializer.Deserialize<BusMessage>(content);

                if (busMessage?.Next is not null)
                {
                    busMessage.Source = busMessage.Destination;
                    busMessage.Destination = busMessage.Next;
                    busMessage.Next = null;

                    ReapplyMessage(busMessage, ct).ConfigureAwait(false);
                }

                _logger.LogInformation($"Получено сообщение: {content} типа {ea.BasicProperties.Type}");

                var outboxEntry = await _outboxDao.QueryAsync(ea.BasicProperties.CorrelationId);

                if (outboxEntry is not null)
                {
                    await _outboxDao.MarkAsSent(outboxEntry);
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            await _channel.BasicConsumeAsync(_queueName, false, consumer);
        }

        private async Task ReapplyMessage(BusMessage busMessage, CancellationToken ct = default)
        {
            var message = JsonSerializer.Serialize(busMessage);

            var body = Encoding.UTF8.GetBytes(message);

            var props = new BasicProperties();
            props.CorrelationId = Guid.NewGuid().ToString();
            props.ContentType = "application/json";
            props.DeliveryMode = DeliveryModes.Persistent;
            props.Expiration = "36000000";
            props.Type = busMessage.Type;

            var outboxEntry = new OutboxEntry
            {
                CorrelationId = props.CorrelationId,
                Destination = busMessage.Destination,
                MessageBody = message,
                MessageContentType = props.ContentType,
                MessageStatus = OutboxEntryStatus.InProcess,
            };

            await _outboxDao.CreateEntry(outboxEntry);

            try
            {
                await _channel
                    .BasicPublishAsync("", busMessage.Destination, true, props, body, ct)
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                await _outboxDao.MarkAsError(outboxEntry);
            }
        }

        public void DisposeAsync()
        {
            _channel.CloseAsync();
            _connection.CloseAsync();
        }
    }
}
