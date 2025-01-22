using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Bus
{
    public class BusService : IBusService
    {
        private readonly string _queueName;

        public BusService(EndpointNameHolder nameHolder)
        {
            _queueName = nameHolder.Name;
        }

        public Task SendMessageAsync(object messageObj, string destination, CancellationToken ct = default)
        {
            var message = JsonSerializer.Serialize(messageObj);

            return SendMessageAsync(message, destination, ct);
        }

        public async Task SendMessageAsync(string message, string destination, CancellationToken ct = default)
        {
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

            var body = Encoding.UTF8.GetBytes(message);

            var props = new BasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = DeliveryModes.Persistent;
            props.Expiration = "36000000";

            await channel
                .BasicPublishAsync("", destination, true, props, body, ct)
                .ConfigureAwait(false);
        }
    }
}
