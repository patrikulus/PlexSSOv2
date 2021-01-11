using System;
using System.Linq;
using System.Reflection;
using PlexSSO.Common.Model;

namespace PlexSSO.Common.Extensions
{
    public static class CliExtensions
    {
        public static (CliArgument, PropertyInfo)[] GetCliArguments(this Type type)
        {
            return type.GetProperties()
                .Select(value => (value.GetCustomAttribute<CliArgument>(), value))
                .Where(value => value.Item1 != null)
                .Distinct()
                .ToArray();
        }
    }
}
