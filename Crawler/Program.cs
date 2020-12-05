using DatabaseAccessLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiotAPIAccessLayer;
using Shared.Configuration;
using System.Threading;

namespace Crawler
{
    class Test
    {
        public int A { get; set; }
        public int B { get; set; }

        public Test(int a, int b) => (A, B) = (a, b);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var config = Configuration.ParseConfig();

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
                        .AddConsole(c =>
                        {
                            c.TimestampFormat = "yyyy/MM/dd - HH:mm:ss | ";
                        });
                })
                .BuildServiceProvider();

            var logger = provider.GetService<ILoggerFactory>().CreateLogger<Program>();

            logger.LogInformation("Crawler instance initialized and connected");

            var crawler = provider.GetService<PointsCrawler>();
            crawler.StartScheduler();

            while (true)
                Thread.Sleep(int.MaxValue);
        }
    }
}
