using System.Threading.Tasks;
using PlexSSO.Common.Model.Types;

namespace PlexSSO.Service.TautulliClient
{
    public interface ITautulliTokenService
    {
        Task<TautulliToken> GetTautulliToken(AccessToken plexToken);
    }
}
