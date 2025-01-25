using Bus.Shared;
using DAO;
using DAO.Outbox;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Bus
{
    public class BusService : IBusService
    {
        private readonly string _queueName;
        private readonly OutboxDao _outboxDao;

        public BusService(EndpointNameHolder nameHolder,
            OutboxDao outboxDao)
        {
            _queueName = nameHolder.Name;
            _outboxDao = outboxDao;
        }

        public Task SendMessageAsync(object messageObj, string destination, string next, CancellationToken ct = default)
        {

            var busMessage = new BusMessage
            {
                Source = _queueName,
                Destination = destination,
                Next = next,
                Type = messageObj.GetType().FullName,
                Body = messageObj,
            };

            return SendMessageAsync(busMessage, ct);
        }

        public async Task SendMessageAsync(BusMessage busMessage, CancellationToken ct = default)
        {
            var stringBody = JsonSerializer.Serialize(busMessage);

            var factory = new ConnectionFactory() {
                HostName = "localhost",
                Port = 5672,
            };

            using var connection = await factory.CreateConnectionAsync(ct);
            using var channel = await connection.CreateChannelAsync(null, ct);

            await channel.QueueDeclareAsync(queue: _queueName,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

            var body = Encoding.UTF8.GetBytes(stringBody);

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
                MessageBody = stringBody,
                MessageContentType = props.ContentType,
                MessageStatus = OutboxEntryStatus.InProcess,
            };

            await _outboxDao.CreateEntry(outboxEntry);

            try
            {
                await channel
                    .BasicPublishAsync("", busMessage.Destination, true, props, body, ct)
                    .ConfigureAwait(false);
            }
            catch (Exception)
            {
                await _outboxDao.MarkAsError(outboxEntry);
            }
        }
    }
}
