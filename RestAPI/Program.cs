using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Configuration;

namespace RestAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureLogging(logging =>
        //        {
        //            logging.AddConsole(c =>
        //            {
        //                c.TimestampFormat = "yyyy/MM/dd - HH:mm:ss | ";
        //            });
        //        })
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            var config = Configuration.ParseConfig();

            var url = config.GetValue<string>("RestAPI:URL");

            return new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseUrls(config.GetValue<string>("RestAPI:URL") ?? "http://localhost:80")
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging
                        .AddConfiguration(hostingContext.Configuration.GetSection("Logging"))
                        .AddConsole(c =>
                        {
                            c.TimestampFormat = "yyyy/MM/dd - HH:mm:ss | ";
                        })
                        .AddDebug()
                        .AddEventSourceLogger();
                })
                .UseStartup<Startup>();
        }
    }
}
