using System.Threading.Tasks;
using PlexSSO.Common.Model.Types;

namespace PlexSSO.Service.PlexClient
{
    public interface IPlexClient
    {
        ServerIdentifier GetLocalServerIdentifier(string path = "Preferences.xml");
        Task<AccessTier> GetAccessTier(ServerIdentifier serverId, AccessToken token);
        Task<User> GetUserInfo(AccessToken token);
    }
}

