using System.Threading.Tasks;
using PlexSSO.Common.Controller;

namespace PlexSSO.Common.Plugin
{
    public interface IServiceAuthenticator
    {
        bool IsServiceType(BaseAuthController controller, (string, string, string) redirectComponents);
        Task<int> AuthenticateWithService(BaseAuthController controller, (string, string, string) redirectComponents);
    }
}
