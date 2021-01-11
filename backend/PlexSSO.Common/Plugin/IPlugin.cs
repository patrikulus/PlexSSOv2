using System;
using Microsoft.Extensions.DependencyInjection;

namespace PlexSSO.Common.Plugin
{
    public interface IPlugin
    {
        Type GetConfigurationType();
        void WireServices(IServiceCollection services);
    }
}
