using Microsoft.Extensions.Logging;
using PushNotifications.Delivery.Pushy.Models;
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

        protected async Task<(HttpResponseMessage Response, PushyResponseModel Data)> ExecuteRequestAsync(HttpRequestMessage request)
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
                                PushyResponseModel deserializedObject = await JsonSerializer.DeserializeAsync<PushyResponseModel>(responseStream, serializerOptions);

                                return (response, deserializedObject);
                            }
                        }
                    }
                    else
                    {
                        using (Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            PushyErrorModel deserializedObject = await JsonSerializer.DeserializeAsync<PushyErrorModel>(responseStream, serializerOptions);

                            if (log.IsEnabled(LogLevel.Error))
                            {
                                log.LogError(deserializedObject.Error);
                            }

                            return (response, new PushyResponseModel() { Error = deserializedObject, Success = false });
                        }
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
