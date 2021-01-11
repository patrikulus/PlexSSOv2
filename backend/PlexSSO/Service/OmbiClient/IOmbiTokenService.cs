using System.Threading.Tasks;
using PlexSSO.Common.Model.Types;

namespace PlexSSO.Service.OmbiClient
{
    public interface IOmbiTokenService
    {
        Task<OmbiToken> GetOmbiToken(AccessToken plexToken);
    }
}
