using PlexSSO.Common.Model;

namespace PlexSSO.Ombi.Model
{
    public class OmbiConfiguration
    {
        public string OmbiPublicHostname { get; set; } = "";
        [CliArgument("c", "cookie_domain")]
        public string CookieDomain { get; set; } = "";
    }
}
