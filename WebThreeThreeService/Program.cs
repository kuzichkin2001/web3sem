using Bus.Extensions;
using DAO.Outbox;
using DAO;
using Configuration;

namespace WebThreeThreeService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddEndpoint("Web3.3");

            // Add data access
            builder.Services.AddSingleton<UnitOfWork>();
            builder.Services.AddTransient<OutboxDao>();
            builder.Services.AddDataAccessConfiguration(builder.Configuration);

            builder.Services.AddHostedService<WebThreeThreeHostedService>();

            var host = builder.Build();
            host.Run();
        }
    }
}