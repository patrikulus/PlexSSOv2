using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PlexSSO.Common.Extensions;
using PlexSSO.Common.Plugin;

namespace PlexSSO.Plugin
{
    public static class PluginLoader
    {
        private const string PLUGIN_DIRECTORY_ARG = "--plugin-directory";

        private static readonly List<IPlugin> _plugins;

        static PluginLoader()
        {
            _plugins = new List<IPlugin>();
            LoadPlugins();
        }

        public static IDictionary<string, string> GetAllCliArguments()
        {
            var dict = new Dictionary<string, string>();
            foreach (var cliArgument in _plugins.SelectMany(plugin => plugin.GetConfigurationType().GetCliArguments()))
            {
                foreach (var arg in cliArgument.Item1.Arguments)
                {
                    dict.Add((arg.Length > 1 ? "--" : "-") + arg, cliArgument.Item1.Arguments[0]);
                }
            }

            dict.Add("-c", "config");
            dict.Add("--config", "config");
            dict.Add(PLUGIN_DIRECTORY_ARG, "plugin");

            return dict;
        }

        public static void WirePlugins(IServiceCollection services)
        {
            foreach (var plugin in _plugins)
            {
                plugin.WireServices(services);
            }
        }

        private static void LoadPlugins()
        {
            var pluginType = typeof(IPlugin);
            var pluginDirectory = GetPluginDirectory();
            foreach (var entry in Directory.EnumerateFileSystemEntries(pluginDirectory, "*.plugin.dll"))
            {
                var assembly = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, entry));
                var plugins = assembly.GetTypes()
                    .Where(type => pluginType.IsAssignableFrom(type))
                    .Select(plugin => (IPlugin) Activator.CreateInstance(plugin));
                
                _plugins.AddRange(plugins);
            }
        }

        private static string GetPluginDirectory()
        {
            var dir = Environment.GetCommandLineArgs()
                .SkipWhile(value => value != PLUGIN_DIRECTORY_ARG)
                .Skip(1)
                .Select(Path.GetFullPath)
                .First();
            return string.IsNullOrWhiteSpace(dir) ? Environment.CurrentDirectory : dir;
        }
    }
}
