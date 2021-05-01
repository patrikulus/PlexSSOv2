using Microsoft.AspNetCore.Mvc;
using PlexSSO.Model;
using PlexSSO.Model.API;
using PlexSSO.Service.Auth;

namespace PlexSSO.Controllers
{
    [ApiController]
    [Route(Constants.ControllerPath)]
    public class ChallengeController : CommonAuthController
    {
        private readonly IAuthValidator _authValidator;

        public ChallengeController(IAuthValidator authValidator)
        {
            _authValidator = authValidator;
        }

        [HttpGet]
        public ActionResult<SsoResponse> Get()
        {
            var response = _authValidator.ValidateAuthenticationStatus(Identity, ServiceName, ServiceUri);
            Response.StatusCode = response.Status;

            if (response.AccessBlocked)
            {
                return LocalRedirect($"/{ServiceName}");
            }

            return response;
        }
    }
}
