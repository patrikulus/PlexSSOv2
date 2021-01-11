using System;
using System.Linq;

namespace PlexSSO.Common.Model
{
    public class CliArgument : Attribute
    {
        public string[] Arguments { get; }
        public CliArgument(params string[] args)
        {
            Arguments = args;
        }

        public override bool Equals(object obj)
        {
            return obj is CliArgument arg && Arguments.SequenceEqual(arg.Arguments);
        }
    }
}
