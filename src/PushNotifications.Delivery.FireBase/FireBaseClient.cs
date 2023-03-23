using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery.FireBase.Models;
using PushNotifications.PushNotifications;
using PushNotifications.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static PushNotifications.Delivery.FireBase.Models.FireBaseResponseModel;

namespace PushNotifications.Delivery.FireBase
{
    public sealed class FireBaseClient : HttpClientBase
    {
        public FireBaseClient(HttpClient client, IOptionsMonitor<FireBaseOptions> monitor, ILogger<FireBaseClient> log) : base(client, monitor, log) { }

        public async Task<SendTokensResult> SendAsync(IEnumerable<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            if (ReferenceEquals(null, tokens) == true) throw new ArgumentNullException(nameof(tokens));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));
            if (tokens.Any() == false) throw new ArgumentException("Tokens are missing");

            const string resource = "fcm/send";

            if (log.IsEnabled(LogLevel.Debug))
                log.LogDebug($"Sending '{tokens.Count()}' PN for notification with body '{notification.NotificationPayload.Body}'");

            List<string> tokensAsStrings = tokens.Select(x => x.ToString()).ToList();
            NotificationPayload payload = notification.NotificationPayload;
            Dictionary<string, object> data = notification.NotificationData;
            string badge = payload.Badge > 0 ? payload.Badge.ToString() : "1";
            var fireBaseSendNotificationModel = new FireBaseSendNotificationModel(payload.Title, payload.Body, payload.Sound, badge);
            var model = new FireBaseSendModel(tokensAsStrings, fireBaseSendNotificationModel, data, notification.ExpiresAt);

            HttpRequestMessage requestMessage = CreateJsonPostRequest(model, resource, notification.Target);
            var result = await ExecuteRequestAsync<FireBaseResponseModel>(requestMessage).ConfigureAwait(false);

            if (result.Response.IsSuccessStatusCode)
            {
                if (result.Data.HasDataFailure())
                {
                    List<FireBaseResponseResultModel> firebaseResponseModel = result.Data.Results;
                    return ExpiredTokensDetector.GetNotRegisteredTokens(tokens.ToList(), firebaseResponseModel);
                }

                return SendTokensResult.Success;
            }
            else
            {
                if (log.IsEnabled(LogLevel.Error))
                {
                    string dataErrors = result.Data.GetDataErrors();
                    log.LogError($"[FireBase] failure: status code '{result.Response.StatusCode}'. PN body '{notification.NotificationPayload.Body}'{Environment.NewLine}{dataErrors}{Environment.NewLine}{result.Response.ReasonPhrase}");
                }

                return SendTokensResult.Failed;
            }
        }

        public async Task<bool> SendToTopic(Topic topic, NotificationForDelivery notification)
        {
            if (ReferenceEquals(null, topic) == true) throw new ArgumentNullException(nameof(topic));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));

            const string resource = "fcm/send";

            if (log.IsEnabled(LogLevel.Debug))
                log.LogDebug($"[FireBase] sending PN to topic: {topic} for notification with body '{notification.NotificationPayload.Body}'");

            var payload = notification.NotificationPayload;
            var data = notification.NotificationData;
            string badge = payload.Badge > 0 ? payload.Badge.ToString() : "1";

            var fireBaseSendNotificationModel = new FireBaseSendNotificationModel(payload.Title, payload.Body, payload.Sound, badge);
            var model = new FireBaseTopicSendModel(topic, fireBaseSendNotificationModel, data, notification.ExpiresAt);

            HttpRequestMessage requestMessage = CreateJsonPostRequest(model, resource, notification.Target);
            var result = await ExecuteRequestAsync<FireBaseResponseModel>(requestMessage).ConfigureAwait(false);

            if (result.Response.IsSuccessStatusCode == false || result.Data.HasDataFailure())
            {
                if (log.IsEnabled(LogLevel.Error))
                {
                    string dataErrors = result.Data.GetDataErrors();
                    log.LogError($"[FireBase] failure: status code '{result.Response.StatusCode}'. PN body '{notification.NotificationPayload.Body}'{Environment.NewLine}{dataErrors}{Environment.NewLine}{result.Response.ReasonPhrase}");
                }

                return false;
            }

            if (log.IsEnabled(LogLevel.Information))
                log.LogInformation($"[FireBase] success: PN with body {notification.NotificationPayload?.Body} was sent to {topic} topic");

            return true;
        }

        public async Task<bool> SubscribeToTopic(SubscriptionToken token, Topic topic, NotificationTarget target)
        {
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));
            if (ReferenceEquals(null, token)) throw new ArgumentNullException(nameof(token));

            const string resource = "/iid/v1:batchAdd";

            if (log.IsEnabled(LogLevel.Debug))
                log.LogDebug($"[FireBase] subscribing '{token.Token}' to topic: `{topic}'");

            var model = new FireBaseSubscriptionModel(token.Token, topic);

            HttpRequestMessage requestMessage = CreateJsonPostRequest(model, resource, target);
            var result = await ExecuteRequestAsync<FireBaseResponseModel>(requestMessage).ConfigureAwait(false);

            if (result.Response.IsSuccessStatusCode == false || result.Data.HasDataFailure())
            {
                if (log.IsEnabled(LogLevel.Error))
                {
                    string dataErrors = result.Data.GetDataErrors();
                    log.LogError($"[FireBase] failure: status code '{result.Response.StatusCode}'. subscription token: '{token.Token}' from topic: {topic}'{Environment.NewLine}{dataErrors}{Environment.NewLine}{result.Response.ReasonPhrase}");
                }

                return false;
            }

            if (log.IsEnabled(LogLevel.Information))
                log.LogInformation($"[FireBase] success: subscribed `{token.Token}` to topic: `{topic}`");

            return true;
        }

        public async Task<bool> UnsubscribeFromTopic(SubscriptionToken token, Topic topic, NotificationTarget target)
        {
            if (ReferenceEquals(null, token)) throw new ArgumentNullException(nameof(token));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            const string resource = "/iid/v1:batchRemove";

            if (log.IsEnabled(LogLevel.Debug))
                log.LogDebug($"[FireBase] unsubscribing '{token.Token}' from topic: `{topic}'");

            var model = new FireBaseSubscriptionModel(token.Token, topic);

            HttpRequestMessage requestMessage = CreateJsonPostRequest(model, resource, target);
            var result = await ExecuteRequestAsync<FireBaseResponseModel>(requestMessage).ConfigureAwait(false);

            if (result.Response.IsSuccessStatusCode == false || result.Data.HasDataFailure())
            {
                if (log.IsEnabled(LogLevel.Error))
                {
                    string dataErrors = result.Data.GetDataErrors();
                    log.LogError($"[FireBase] failure: status code '{result.Response.StatusCode}'. subscription token: '{token.Token}' from topic: {topic}'{Environment.NewLine}{dataErrors}{Environment.NewLine}{result.Response.ReasonPhrase}");
                }

                return false;
            }

            if (log.IsEnabled(LogLevel.Information))
                log.LogInformation($"[FireBase] success: unsubscribed `{token.Token}` from topic: `{topic}`");

            return true;
        }
    }
}
