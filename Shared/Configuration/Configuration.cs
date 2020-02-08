using Microsoft.Extensions.Configuration;
using System.IO;

namespace Shared.Configuration
{
    public class Configuration
    {
        public static IConfiguration ParseConfig() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables(prefix: "SC_")
                .Build();
    }
}
