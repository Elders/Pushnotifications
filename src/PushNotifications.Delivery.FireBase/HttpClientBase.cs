using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PushNotifications.Contracts.PushNotifications.Delivery;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static PushNotifications.Delivery.FireBase.FireBaseOptions;

namespace PushNotifications.Delivery.FireBase
{
    public abstract class HttpClientBase
    {
        private readonly HttpClient client;
        protected readonly ILogger log;
        private readonly JsonSerializerOptions serializerOptions;
        private FireBaseOptions options;

        public HttpClientBase(HttpClient client, IOptionsMonitor<FireBaseOptions> monitor, ILogger logger)
        {
            this.client = client;
            this.log = logger;
            this.serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            options = monitor.CurrentValue;
            monitor.OnChange(OptionsChanged);

            client.BaseAddress = new Uri(options.BaseAddress);
        }

        protected async Task<(HttpResponseMessage Response, T Data)> ExecuteRequestAsync<T>(HttpRequestMessage request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            using (HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        T responseObject = await JsonSerializer.DeserializeAsync<T>(contentStream, serializerOptions).ConfigureAwait(false);
                        return (response, responseObject);
                    }
                }
                else
                {
                    string errorResponseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    log.LogError(errorResponseString);

                    return (response, default);
                }
            }
        }

        protected HttpRequestMessage CreateJsonPostRequest<T>(T model, string resource, NotificationTarget target)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, resource) { Content = new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, ContentType.Json) };

            AuthorizationKey key = options.GetKey(target.Tenant, target.Application);
            if (key is null == false)
            {
                httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", $"key={key.ServerKey}");
                return httpRequestMessage;
            }

            throw new Exception($"FireBase authorization server key for tenant {target.Tenant} and application {target.Application} was not found.");
        }

        internal static class ContentType
        {
            public static string Json = "application/json";
        }


        private void OptionsChanged(FireBaseOptions newOptions)
        {
            options = newOptions;
            client.BaseAddress = new Uri(options.BaseAddress);
        }
    }

    internal class ApiException : Exception
    {
        public int StatusCode { get; set; }

        public string Content { get; set; }
    }
}
