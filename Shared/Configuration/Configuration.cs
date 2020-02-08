using Microsoft.Extensions.Configuration;
using System.IO;

namespace Shared.Configuration
{
    public class Configuration
    {
        public static IConfiguration ParseConfig(string productionConfig = "appsettings.json") =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(productionConfig, optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables(prefix: "SC_")
                .Build();
    }
}
