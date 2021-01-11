using System;
using System.Collections.Generic;
using System.Security.Claims;
using PlexSSO.Common.Model.Types;

namespace PlexSSO.Common.Model
{
    public class Identity
    {
        private static readonly Username DEFAULT_USERNAME = new Username(string.Empty);

        public AccessTier PlexAccessTier { get; set; } = AccessTier.NoAccess;
        public AccessToken PlexAccessToken { get; set; }
        public ServerIdentifier PlexServerIdentifier { get; set; }
        public Username Username { get; set; } = DEFAULT_USERNAME;
        public Email Email { get; set; }
        public Thumbnail Thumbnail { get; set; } = new Thumbnail("https://about:blank");

        private readonly bool _hasIdentity;

        public Identity(IEnumerable<Claim> claims)
        {
            foreach (var claim in claims)
            {
                switch (claim.Type)
                {
                    case Constants.AccessTierClaim:
                        PlexAccessTier = (AccessTier) Enum.Parse(typeof(AccessTier), claim.Value);
                        _hasIdentity = true;
                        break;
                    case Constants.AccessTokenClaim:
                        PlexAccessToken = new AccessToken(claim.Value);
                        break;
                    case Constants.ServerIdentifierClaim:
                        PlexServerIdentifier = new ServerIdentifier(claim.Value);
                        break;
                    case Constants.UsernameClaim:
                        Username = new Username(claim.Value);
                        break;
                    case Constants.EmailClaim:
                        Email = new Email(claim.Value);
                        break;
                    case Constants.ThumbnailClaim:
                        Thumbnail = new Thumbnail(claim.Value);
                        break;
                }
            }
        }

        public List<Claim> AsClaims()
        {
            return new List<Claim>
            {
                new Claim(Constants.AccessTierClaim, PlexAccessTier.ToString()),
                new Claim(Constants.AccessTokenClaim, PlexAccessToken.Value),
                new Claim(Constants.ServerIdentifierClaim, PlexServerIdentifier.Value),
                new Claim(Constants.UsernameClaim, Username.Value),
                new Claim(Constants.EmailClaim, Email.Value),
                new Claim(Constants.ThumbnailClaim, Thumbnail.Value)
            };
        }

        public bool HasIdentity()
        {
            return _hasIdentity;
        }
    }
}
