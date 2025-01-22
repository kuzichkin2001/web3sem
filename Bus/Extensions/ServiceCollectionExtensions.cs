using Microsoft.Extensions.DependencyInjection;

namespace Bus.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEndpoint(this IServiceCollection services, string queueName)
            => services
            .AddSingleton(new EndpointNameHolder { Name = queueName })
            .AddSingleton<IBusConsumer, BusConsumer>()
            .AddTransient<IBusService, BusService>();
    }
}
