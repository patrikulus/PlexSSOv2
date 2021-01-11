using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PlexSSO.Plugin;

namespace PlexSSO
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddCommandLine(args, PluginLoader.GetAllCliArguments());
                })
                .ConfigureKestrel((context, options) => options.AddServerHeader = false)
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:4200/")
                .Build()
                .Run();
        }
    }
}
