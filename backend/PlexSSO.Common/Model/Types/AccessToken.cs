using System;

namespace PlexSSO.Common.Model.Types
{
    public class AccessToken : ValueType<string>
    {
        public AccessToken(string token) : base(token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Provided token cannot be null or empty");
            }
        }
    }
}
