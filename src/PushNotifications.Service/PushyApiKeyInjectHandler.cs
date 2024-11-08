using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PushNotifications.Delivery.Pushy;

namespace PushNotifications.Service
{
    public class PushyApiKeyInjectHandler : DelegatingHandler
    {
        private const string ApiKeyName = "api_key";

        PushyOptions options;

        public PushyApiKeyInjectHandler(IOptions<PushyOptions> monitor)
        {
            options = monitor.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.PathAndQuery.Contains(ApiKeyName) == false)
            {
                request.RequestUri = new Uri($"{request.RequestUri.OriginalString}?{ApiKeyName}={options.ApiKey}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
