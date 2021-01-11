using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using PlexSSO.Common.Extensions;

namespace PlexSSO.Common.Service.Config
{
    public class ConfigurationService : IConfigurationService
    {
        private const string CONFIG_KEY = "config";
        private const string CONFIG_FILE = "config.json";

        private readonly IDictionary<Type, object> _configurationObjects;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
            _configurationObjects = new Dictionary<Type, object>();
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                },
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }
        
        public string GetConfigurationFile()
        {
            return _configuration[CONFIG_KEY] ?? Path.Combine(Environment.CurrentDirectory, CONFIG_FILE);
        }

        public string GetConfigurationDirectory()
        {
            return Path.GetDirectoryName(GetConfigurationFile());
        }

        public T GetConfiguration<T>()
        {
            if (!_configurationObjects.TryGetValue(typeof(T), out var val))
            {
                _configurationObjects[typeof(T)] = LoadConfigFile<T>();
            }
            return (T) val;
        }

        private T LoadConfigFile<T>()
        {
            var configFile = GetConfigurationFile();
            T config;
            if (File.Exists(configFile))
            {
                config = JsonSerializer.Deserialize<T>(File.ReadAllText(configFile), _jsonSerializerOptions);
            }
            else
            {
                config = Activator.CreateInstance<T>();
            }
            UpdateConfigWithCliOptions(config);

            return config;
        }

        private void UpdateConfigWithCliOptions<T>(T config)
        {
            foreach (var (cliArgument, property) in config.GetType().GetCliArguments())
            {
                object value = _configuration[cliArgument.Arguments[0]];
                var propertyType = property.GetType();
                if (propertyType != typeof(string))
                {
                    var converter = TypeDescriptor.GetConverter(propertyType);
                    if (converter == null)
                    {
                        throw new Exception("Cannot convert argument");
                    }

                    value = converter.ConvertFromString((string)value);
                }

                property.SetValue(config, value);
            }
        }
    }
}
