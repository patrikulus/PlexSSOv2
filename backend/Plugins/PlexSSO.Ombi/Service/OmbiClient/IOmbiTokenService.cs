using System.Threading.Tasks;
using PlexSSO.Common.Model.Types;
using PlexSSO.Ombi.Model;

namespace PlexSSO.Ombi.Service.OmbiClient
{
    public interface IOmbiTokenService
    {
        Task<OmbiToken> GetOmbiToken(AccessToken plexToken);
    }
}
