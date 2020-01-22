using DatabaseAccessLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiotAPIAccessLayer;
using System.IO;
using System.Threading;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables(prefix: "MPS_")
                .Build();


            var provider = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddDbContext<DatabaseContext>()
                .AddSingleton<DatabaseAccess>()
                .AddSingleton<RiotAPIWrapper>()
                .AddSingleton<PointsCrawler>()
                .AddLogging(opt =>
                {
                    opt
                        .AddConfiguration(config.GetSection("Logging"))
                        .AddConsole();
                })
                .BuildServiceProvider();

            var logger = provider.GetService<ILoggerFactory>().CreateLogger<Program>();

            logger.LogInformation("Crawler instance initialized and connected");

            var crawler = provider.GetService<PointsCrawler>();
            crawler.StartLoop();

            while (true)
                Thread.Sleep(int.MaxValue);
        }
    }
}
