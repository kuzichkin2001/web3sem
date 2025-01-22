using Bus.Extensions;

namespace WebThreeOneService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddEndpoint("Web3.1");
            builder.Services.AddHostedService<WebThreeOneHostedService>();

            var host = builder.Build();
            host.Run();
        }
    }
}