using Bus;

namespace WebThreeOneService
{
    public class WebThreeOneHostedService : BackgroundService
    {
        private readonly ILogger<WebThreeOneHostedService> _logger;
        private readonly IBusConsumer _consumer;

        public WebThreeOneHostedService(ILogger<WebThreeOneHostedService> logger,
            IBusConsumer consumer)
        {
            _logger = logger;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            if (!_consumer.ChannelConstructed)
            {
                await _consumer.InitializeConnection();
            }

            await _consumer.ListenAsync(ct);
        }
    }
}
