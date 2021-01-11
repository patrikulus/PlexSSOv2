using System;
using Microsoft.Extensions.DependencyInjection;
using PlexSSO.Common.Plugin;
using PlexSSO.Ombi.Model;
using PlexSSO.Ombi.Service;
using PlexSSO.Ombi.Service.OmbiClient;

namespace PlexSSO.Ombi.Plugin
{
    public class PluginDescriptor : IPlugin
    {
        public Type GetConfigurationType() => typeof(OmbiConfiguration);

        public void WireServices(IServiceCollection services)
        {
            services.AddSingleton<IOmbiTokenService, OmbiTokenService>();
            services.AddSingleton<IServiceAuthenticator, OmbiServiceAuthenticator>();
        }
    }
}
