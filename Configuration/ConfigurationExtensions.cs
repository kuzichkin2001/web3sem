using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Configuration.Options;

namespace Configuration
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddDataAccessConfiguration(this IServiceCollection services, IConfigurationRoot config)
            => services.Configure<DataAccessOptions>(config.GetSection(DataAccessOptions.SectionName));
    }
}
