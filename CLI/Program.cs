using CLI.Modules;
using CommandLine;
using Crawler;
using DatabaseAccessLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiotAPIAccessLayer;
using Shared.Configuration;

namespace CLI
{
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
                .AddTransient<SummonerModule>()
                .AddTransient<PointsModule>()
                .AddTransient<PointsLogModule>()
                .AddTransient<PointsCrawler>()
                .AddLogging(opt =>
                {
                    opt
                        .AddConfiguration(config.GetSection("Logging"))
                        .AddConsole();
                })
                .BuildServiceProvider();

            Parser.Default.ParseArguments<SummonerModuleOptions, PointsModuleOptions, PointsLogModuleOptions>(args)
                .MapResult(
                    (SummonerModuleOptions opts) => provider.GetService<SummonerModule>().Exec(opts),
                    (PointsModuleOptions opts) => provider.GetService<PointsModule>().Exec(opts),
                    (PointsLogModuleOptions opts) => provider.GetService<PointsLogModule>().Exec(opts),
                    errs => 1
                );

        }
    }
}
