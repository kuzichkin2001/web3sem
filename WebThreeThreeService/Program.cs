using Bus.Extensions;

namespace WebThreeThreeService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddEndpoint("Web3.3");
            builder.Services.AddHostedService<WebThreeThreeHostedService>();

            var host = builder.Build();
            host.Run();
        }
    }
}