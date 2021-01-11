using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PlexSSO.Common.Extensions;
using PlexSSO.Common.Model.Types;
using PlexSSO.Ombi.Model;

namespace PlexSSO.Ombi.Service.OmbiClient
{
    public class OmbiTokenService : IOmbiTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfigurationService _configurationService;

        public OmbiTokenService(IHttpClientFactory clientFactory,
                                IConfigurationService configurationService)
        {
            _httpClient = clientFactory.CreateClient();
            _configurationService = configurationService;
        }

        public async Task<OmbiToken> GetOmbiToken(AccessToken plexToken)
        {

            var request = new HttpRequestMessage(HttpMethod.Post,
                _configurationService.GetOmbiUrl() + "/api/v1/token/plextoken")
            {
                Content = new StringContent($"{{\"plexToken\":\"{plexToken.Value}\"}}", Encoding.UTF8,
                    "application/json")
            };
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var ombiResponse = JsonSerializer.Deserialize<OmbiTokenResponse>(json, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true
            });

            return string.IsNullOrWhiteSpace(ombiResponse?.AccessToken)
                ? null
                : new OmbiToken(ombiResponse.AccessToken);
        }
    }
}

