using CLI.Modules;
using CommandLine;
using DatabaseAccessLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RiotAPIAccessLayer;
using Shared.Configuration;
using Microsoft.Extensions.Logging;

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
                .AddLogging(opt =>
                {
                    opt
                        .AddConfiguration(config.GetSection("Logging"))
                        .AddConsole();
                })
                .BuildServiceProvider();

            CommandLine.Parser.Default.ParseArguments<SummonerModuleOptions>(args)
                .MapResult(
                    (SummonerModuleOptions opts) => provider.GetService<SummonerModule>().Exec(opts),
                    errs => 1
                );

        }
    }
}
