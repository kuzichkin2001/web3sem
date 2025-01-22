using Bus.Extensions;

namespace WebThreeTwoService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddEndpoint("Web3.2");
            builder.Services.AddHostedService<WebThreeTwoHostedService>();

            var host = builder.Build();
            host.Run();
        }
    }
}