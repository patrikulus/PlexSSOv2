using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlexSSO.Common.Controller;
using PlexSSO.Common.Model.Types;
using PlexSSO.Model;
using PlexSSO.Service.Auth;
using PlexSSO.Service.Config;
using PlexSSO.Service.PlexClient;
using ServerIdentifier = PlexSSO.Service.PlexClient.ServerIdentifier;

namespace PlexSSO.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class LoginController : BaseAuthController
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IPlexClient _plexClient;
        private readonly IAuthValidator _authValidator;
        private readonly ServerIdentifier serverIdentifier;

        public LoginController(ILogger<LoginController> logger,
                               IPlexClient plexClient,
                               IAuthValidator authValidator,
                               IConfigurationService configuration)
        {
            _logger = logger;
            _plexClient = plexClient;
            _authValidator = authValidator;

            var id = configuration.GetConfig().ServerIdentifier;
            if (id == null)
            {
                var pref = configuration.GetConfig().PlexPreferencesFile;
                serverIdentifier = plexClient.GetLocalServerIdentifier(pref ?? "Preferences.xml");
            }
            else
            {
                serverIdentifier = new ServerIdentifier(id);
            }
        }

        [HttpPost]
        public async Task<SsoResponse> Login([FromBody] LoginPost data)
        {
            try
            {
                Identity.PlexAccessToken = new AccessToken(data.Token);
                if (!Identity.HasIdentity())
                {
                    Identity.PlexAccessTier = await _plexClient.GetAccessTier(serverIdentifier, Identity.PlexAccessToken);
                }

                if (Identity.PlexAccessTier == AccessTier.Failure)
                {
                    var loginFailureResponse = _authValidator.ValidateAuthenticationStatus(
                        AccessTier.NoAccess,
                        false,
                        GetServiceName(),
                        GetServiceUri(),
                        null);
                    Response.StatusCode = loginFailureResponse.Status;
                    return loginFailureResponse;
                }

                var user = await _plexClient.GetUserInfo(Identity.PlexAccessToken);

                Identity.Email = user.Email;
                Identity.Thumbnail = user.Thumbnail;
                Identity.Username = user.Username;

                var identity = new ClaimsIdentity(
                    Identity.AsClaims(),
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    authProperties
                );

                var response = _authValidator.ValidateAuthenticationStatus(
                    Identity.PlexAccessTier,
                    true,
                    GetServiceName(),
                    GetServiceUri(),
                    user.Username);

                Response.StatusCode = response.Status;
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to log user in", e);
                var unhandledErrorResponse = _authValidator.ValidateAuthenticationStatus(
                    AccessTier.NoAccess,
                    false,
                    GetServiceName(),
                    GetServiceUri(),
                    null,
                    true);
                Response.StatusCode = unhandledErrorResponse.Status;
                return unhandledErrorResponse;
            }
        }
    }
}
