using System.Linq;
using Microsoft.Extensions.Logging;
using PlexSSO.Common.Extensions;
using PlexSSO.Common.Model.Types;
using PlexSSO.Model;
using PlexSSO.Service.Config;
using static PlexSSO.Model.PlexSsoConfig;

namespace PlexSSO.Service.Auth
{
    public class AuthenticationValidator : IAuthValidator
    {
        private readonly IConfigurationService _configurationService;
        private readonly ILogger<AuthenticationValidator> _logger;

        public AuthenticationValidator(IConfigurationService configurationService,
                                       ILogger<AuthenticationValidator> logger)
        {
            _configurationService = configurationService;
            _logger = logger;
        }

        public SsoResponse ValidateAuthenticationStatus(
            AccessTier accessTier,
            bool loggedIn,
            ServiceName serviceName,
            ServiceUri serviceUri,
            Username userName,
            bool failuresHaveOccurred = false
        )
        {
            if (serviceName == null || serviceUri == null || userName == null)
            {
                _logger.LogWarning("Some properties which should not be null/empty were null/empty.\n" +
                    "Did you forget to add a header in your reverse proxy?\n" +
                    $"\tserviceName = {serviceName}\n" +
                    $"\tserviceUri = {serviceUri}\n" +
                    $"\tuserName = {userName}");
            }

            var rules = _configurationService.GetAccessControls(serviceName)
                .Where(rule => rule.Path == null || serviceUri.Value.StartsWith(rule.Path));

            var numRules = 0;
            foreach (var rule in rules)
            {
                var block = rule.ControlType == ControlType.Allow ?
                    accessTier.IsHigherTierThan(rule.MinimumAccessTier ?? AccessTier.Failure) :
                    accessTier.IsLowerTierThan(rule.MinimumAccessTier ?? AccessTier.NoAccess);
                
                if (rule.Exempt.Contains(userName?.Value))
                {
                    block = !block;
                }

                if (block)
                {
                    return new SsoResponse(
                        !failuresHaveOccurred,
                        loggedIn,
                        true,
                        AccessTier.NoAccess,
                        403,
                        string.IsNullOrWhiteSpace(rule.BlockMessage) ? _configurationService.GetConfig().DefaultAccessDeniedMessage : rule.BlockMessage
                    );
                }

                numRules++;
            }

            var globalBlocked = numRules == 0 && accessTier == AccessTier.NoAccess;
            var message = "";
            var status = 200;
            if (globalBlocked)
            {
                if (failuresHaveOccurred)
                {
                    status = 400;
                    message = "An error occurred";
                }
                else if (loggedIn)
                {
                    status = 403;
                    message = _configurationService.GetConfig().DefaultAccessDeniedMessage;
                }
                else
                {
                    status = 401;
                    message = "Login Required";
                }
            }

            return new SsoResponse(
                !failuresHaveOccurred,
                loggedIn,
                globalBlocked,
                accessTier,
                status,
                message
            );
        }
    }

}
