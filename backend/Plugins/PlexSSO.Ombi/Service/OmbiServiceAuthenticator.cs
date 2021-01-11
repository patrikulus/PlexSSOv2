using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PlexSSO.Common.Controller;
using PlexSSO.Common.Plugin;
using PlexSSO.Common.Service.Config;
using PlexSSO.Ombi.Model;
using PlexSSO.Ombi.Service.OmbiClient;

namespace PlexSSO.Ombi.Service
{
    public class OmbiServiceAuthenticator : IServiceAuthenticator
    {
        private readonly IOmbiTokenService _ombiTokenService;
        private readonly IConfigurationService _configurationService;

        public OmbiServiceAuthenticator(IOmbiTokenService ombiTokenService,
                                        IConfigurationService configurationService)
        {
            _ombiTokenService = ombiTokenService;
            _configurationService = configurationService;
        }

        public bool IsServiceType(BaseAuthController controller,
                                  (string, string, string) redirectComponents)
        {
            return _configurationService.GetConfiguration<OmbiConfiguration>().OmbiPublicHostname?.Contains(redirectComponents.Item2) ?? false;
        }

        public async Task<int> AuthenticateWithService(BaseAuthController controller,
                                                       (string, string, string) redirectComponents)
        {
            var (protocol, host, path) = redirectComponents;
            var ombiToken = await _ombiTokenService.GetOmbiToken(controller.Identity.PlexAccessToken);
            controller.Response.Headers.Add("Location", protocol + host + "/auth/cookie");
            controller.Response.Cookies.Append("Auth", ombiToken.Value, new CookieOptions
            {
                HttpOnly = false,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                Expires = DateTimeOffset.Now.AddHours(1),
                Domain = _configurationService.GetConfiguration<OmbiConfiguration>().CookieDomain,
                Path = "/",
                Secure = false
            });
            return 302;
        }
    }
}
