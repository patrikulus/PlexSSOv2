using Microsoft.AspNetCore.Mvc;
using PlexSSO.Common.Controller;
using PlexSSO.Model;
using PlexSSO.Service.Auth;

namespace PlexSSO.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class SsoController : BaseAuthController
    {
        private readonly IAuthValidator _authValidator;

        public SsoController(IAuthValidator authValidator)
        {
            _authValidator = authValidator;
        }

        [HttpGet]
        public SsoResponse Get()
        {
            var response = _authValidator.ValidateAuthenticationStatus(
                Identity.PlexAccessTier,
                Identity.HasIdentity(),
                GetServiceName(),
                GetServiceUri(),
                Identity.Username);
            Response.StatusCode = response.Status;
            return response;
        }
    }
}
