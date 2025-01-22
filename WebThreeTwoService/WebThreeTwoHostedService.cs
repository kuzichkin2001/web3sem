using Bus;

namespace WebThreeTwoService
{
    public class WebThreeTwoHostedService : BackgroundService
    {
        private readonly ILogger<WebThreeTwoHostedService> _logger;
        private readonly IBusConsumer _consumer;

        public WebThreeTwoHostedService(ILogger<WebThreeTwoHostedService> logger,
            IBusConsumer consumer)
        {
            _logger = logger;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_consumer.ChannelConstructed)
            {
                await _consumer.InitializeConnection();
            }

            await _consumer.ListenAsync(stoppingToken);
        }
    }
}
