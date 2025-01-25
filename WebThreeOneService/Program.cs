using Bus.Extensions;
using DAO.Outbox;
using DAO;
using Configuration;

namespace WebThreeOneService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddEndpoint("Web3.1");

            // Add data access
            builder.Services.AddSingleton<UnitOfWork>();
            builder.Services.AddTransient<OutboxDao>();
            builder.Services.AddDataAccessConfiguration(builder.Configuration);
            builder.Services.AddHostedService<WebThreeOneHostedService>();

            var host = builder.Build();
            host.Run();
        }
    }
}