using Microsoft.Extensions.Logging;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery.Pushy.Models;
using PushNotifications.PushNotifications;
using PushNotifications.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PushNotifications.Delivery.Pushy
{
    public sealed class PushyClient : HttpClientBase
    {
        public PushyClient(HttpClient client, ILogger<PushyClient> log) : base(client, log) { }

        public async Task<SendTokensResult> SendAsync(IEnumerable<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            if (tokens is null == true) throw new ArgumentNullException(nameof(tokens));
            if (notification is null == true) throw new ArgumentNullException(nameof(notification));
            if (tokens.Any() == false) throw new ArgumentException("Tokens are missing");

            const string resource = "push";

            if (log.IsEnabled(LogLevel.Debug))
                log.LogDebug($"Sending '{tokens.Count()}' notification(s) with body '{notification.NotificationPayload.Body}'");

            List<string> tokensAsStrings = tokens.Select(x => x.ToString()).ToList();
            NotificationPayload payload = notification.NotificationPayload;
            Dictionary<string, object> data = notification.NotificationData;
            int badge = payload.Badge > 0 ? payload.Badge : 1;
            var model = new PushySendNotificationModel(payload.Title, payload.Body, payload.Sound, badge);
            var send = new PushySendModel(tokensAsStrings, model, notification.NotificationData, notification.ExpiresAt, notification.ContentAvailable);

            HttpRequestMessage requestMessage = CreateJsonPostRequest(send, resource);
            var result = await ExecuteRequestAsync<PushyResponseModel>(requestMessage).ConfigureAwait(false);

            if (result.Response.IsSuccessStatusCode)
            {
                if (log.IsEnabled(LogLevel.Information))
                    log.LogInformation($"Notification with body {notification.NotificationPayload?.Body} was sent");
                return SendTokensResult.Success;
            }
            else
            {
                if (log.IsEnabled(LogLevel.Error))
                {
                    string dataErrors = result.Data?.Error;
                    log.LogError($"{result.Response.StatusCode} Failed to sent notification '{notification.NotificationPayload.Body}'{Environment.NewLine}{dataErrors}{Environment.NewLine}{result.Response.ReasonPhrase}");
                }

                return SendTokensResult.Failed;
            }
        }

        public async Task<bool> SendToTopic(Topic topic, NotificationForDelivery notification)
        {
            if (topic is null == true) throw new ArgumentNullException(nameof(topic));
            if (notification is null == true) throw new ArgumentNullException(nameof(notification));

            const string resource = "push";

            if (log.IsEnabled(LogLevel.Debug))
                log.LogDebug($"Sending notification to topic: {topic} for notification with body '{notification.NotificationPayload.Body}'");

            var payload = notification.NotificationPayload;
            int badge = payload.Badge > 0 ? payload.Badge : 1;
            var pushySendNotificationModel = new PushySendNotificationModel(payload.Title, payload.Body, payload.Sound, badge);
            var model = new PushySendToTopicModel(topic, pushySendNotificationModel, notification.NotificationData, notification.ExpiresAt, notification.ContentAvailable);

            HttpRequestMessage requestMessage = CreateJsonPostRequest(model, resource);
            var result = await ExecuteRequestAsync<PushyResponseModel>(requestMessage).ConfigureAwait(false);

            if (result.Response.IsSuccessStatusCode == false)
            {
                if (log.IsEnabled(LogLevel.Error))
                {
                    string dataErrors = result.Data.Error;
                    log.LogError($"{result.Response.StatusCode} Failed to sent notification '{notification.NotificationPayload.Body}'{Environment.NewLine}{dataErrors}{Environment.NewLine}{result.Response.ReasonPhrase}");
                }

                return false;
            }

            if (log.IsEnabled(LogLevel.Information))
                log.LogInformation($"Notification with body {notification.NotificationPayload?.Body} was sent to {topic} topic");

            return true;
        }

        public async Task<bool> SubscribeToTopic(SubscriptionToken token, Topic topic)
        {
            if (topic is null) throw new ArgumentNullException(nameof(topic));
            if (token is null) throw new ArgumentNullException(nameof(token));

            const string resource = "devices/subscribe";

            if (log.IsEnabled(LogLevel.Debug))
                log.LogDebug($"Subscribing '{token.Token}' to topic: `{topic}'");

            var model = new PushyTopicSubscriptionModel(token.Token, topic);

            HttpRequestMessage requestMessage = CreateJsonPostRequest(model, resource);
            var result = await ExecuteRequestAsync<PushyResponseModel>(requestMessage).ConfigureAwait(false);

            if (result.Response.IsSuccessStatusCode == false)
            {
                if (log.IsEnabled(LogLevel.Error))
                {
                    string dataErrors = result.Data.Error;
                    log.LogError($"{result.Response.StatusCode} - Failed to subscribe to topic. Token: '{token.Token}', Topic: {topic}'{Environment.NewLine}{dataErrors}{Environment.NewLine}{result.Response.ReasonPhrase}");
                }

                return false;
            }

            if (log.IsEnabled(LogLevel.Information))
                log.LogInformation($"Subscribed token {token.Token} to topic '{topic}'");

            return true;
        }

        public async Task<bool> UnsubscribeFromTopic(SubscriptionToken token, Topic topic)
        {
            if (topic is null) throw new ArgumentNullException(nameof(topic));
            if (token is null) throw new ArgumentNullException(nameof(token));

            const string resource = "devices/unsubscribe";

            if (log.IsEnabled(LogLevel.Debug))
                log.LogDebug($"Unsubscribing '{token.Token}' from topic: `{topic}'");

            var model = new PushyTopicSubscriptionModel(token.Token, topic);

            HttpRequestMessage requestMessage = CreateJsonPostRequest(model, resource);
            var result = await ExecuteRequestAsync<PushyResponseModel>(requestMessage).ConfigureAwait(false);

            if (result.Response.IsSuccessStatusCode == false)
            {
                if (log.IsEnabled(LogLevel.Error))
                {
                    string dataErrors = result.Data.Error;
                    log.LogError($"{result.Response.StatusCode} - Failed to unsubscribe from topic. Token: '{token.Token}', Topic: {topic}'{Environment.NewLine}{dataErrors}{Environment.NewLine}{result.Response.ReasonPhrase}");
                }

                return false;
            }

            if (log.IsEnabled(LogLevel.Information))
                log.LogInformation($"Unsubscribed token {token.Token} to topic '{topic}'");

            return true;
        }
    }
}
