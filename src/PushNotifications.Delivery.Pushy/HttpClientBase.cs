using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace PushNotifications.Delivery.Pushy
{
    public abstract class HttpClientBase
    {
        private readonly HttpClient client;
        protected readonly ILogger<HttpClientBase> log;
        private readonly JsonSerializerOptions serializerOptions;

        public HttpClientBase(HttpClient client, ILogger<HttpClientBase> log)
        {
            this.client = client;
            this.log = log;

            serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        protected async Task<(HttpResponseMessage Response, T Data)> ExecuteRequestAsync<T>(HttpRequestMessage request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            try
            {
                using (HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content == null == false)
                        {
                            using (Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            {
                                T deserializedObject = await JsonSerializer.DeserializeAsync<T>(responseStream, serializerOptions);

                                return (response, deserializedObject);
                            }
                        }
                    }

                    if (log.IsEnabled(LogLevel.Error))
                    {
                        string errorResponseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        log.LogError(errorResponseString);
                    }

                    return (response, default);
                }
            }
            catch (Exception exception)
            {
                log.LogError(exception, "Unhandled exception when calling the API");
                return (default, default);
            }
        }

        protected HttpRequestMessage CreateJsonPostRequest<T>(T model, string resource)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, resource) { Content = new StringContent(JsonSerializer.Serialize(model), System.Text.Encoding.UTF8, MediaTypeNames.Application.Json) };

            return httpRequestMessage;
        }
    }
}
