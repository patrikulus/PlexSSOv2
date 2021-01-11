using Microsoft.AspNetCore.Mvc;
using PlexSSO.Common.Model;
using PlexSSO.Common.Model.Types;

namespace PlexSSO.Common.Controller
{
    public abstract class BaseAuthController : ControllerBase
    {
        private Identity _identity;

        public Identity Identity => _identity ??= new Identity(User.Claims);

        public HttpProtocol GetProtocol()
        {
            var headerValue = GetFirstHeader(
                Constants.UpstreamProtocolHeader,
                Constants.ForwardedProtoHeader,
                Constants.FrontEndHttpsHeader,
                Constants.ForwardedProtocolHeader,
                Constants.ForwardedSslHeader,
                Constants.UrlSchemeHeader
            );
            switch (headerValue)
            {
                case "on":
                case "https":
                    return HttpProtocol.Https;
                case "off":
                case "http":
                    return HttpProtocol.Http;
                default:
                    goto case "on";
            }
        }

        public string GetFirstHeader(params string[] keys)
        {
            foreach (var key in keys)
            {
                if (HttpContext.Request.Headers.ContainsKey(key))
                {
                    return HttpContext.Request.Headers[key][0];
                }
            }
            return null;
        }

        private string GetHeader(string key, string defaultValue)
        {
            if (!HttpContext.Request.Headers.TryGetValue(key, out var headerValue))
            {
                headerValue = defaultValue;
            }
            return headerValue;
        }

        public ServiceName GetServiceName()
        {
            var serviceName = GetHeader(Constants.SsoServiceHeader, null);
            return string.IsNullOrWhiteSpace(serviceName) ? null : new ServiceName(serviceName);
        }

        public ServiceUri GetServiceUri()
        {
            var serviceUri = GetHeader(Constants.SsoOrigionalUriHeader, null);
            return string.IsNullOrWhiteSpace(serviceUri) ? null : new ServiceUri(serviceUri);
        }
    }
}
