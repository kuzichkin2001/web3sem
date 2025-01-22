using Bus;

namespace WebThreeThreeService
{
    public class WebThreeThreeHostedService : BackgroundService
    {
        private readonly ILogger<WebThreeThreeHostedService> _logger;
        private readonly IBusConsumer _consumer;

        public WebThreeThreeHostedService(ILogger<WebThreeThreeHostedService> logger,
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
