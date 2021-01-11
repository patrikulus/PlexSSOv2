using PlexSSO.Common.Model.Types;
using PlexSSO.Model;

namespace PlexSSO.Service.Auth
{
    public interface IAuthValidator
    {
        SsoResponse ValidateAuthenticationStatus(
            AccessTier accessTier,
            bool loggedIn,
            ServiceName serviceName,
            ServiceUri serviceUri,
            Username userName,
            bool failuresHaveOccurred = false
        );
    }
}
