using System;
using System.Collections.Generic;
using System.Linq;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery.Pushy.Logging;
using PushNotifications.Delivery.Pushy.Models;
using RestSharp;
using RestSharp.Serializers;

namespace PushNotifications.Delivery.Pushy
{
    public class PushyDelivery : IPushNotificationDelivery
    {
        static ILog log = LogProvider.GetLogger(typeof(PushyDelivery));

        readonly string serverKey;

        readonly IRestClient restClient;

        readonly ISerializer serializer;

        IPushNotificationAggregator pushNotificationAggregator;

        public PushyDelivery(IRestClient restClient, ISerializer serializer, string serverKey)
        {
            if (ReferenceEquals(null, restClient) == true) throw new ArgumentNullException(nameof(restClient));
            if (ReferenceEquals(null, serializer) == true) throw new ArgumentNullException(nameof(serializer));
            if (string.IsNullOrEmpty(serverKey) == true) throw new ArgumentNullException(nameof(serverKey));

            this.restClient = restClient;
            this.serializer = serializer;
            this.serverKey = serverKey;
        }

        public void UseAggregator(IPushNotificationAggregator pushNotificationAggregator)
        {
            if (ReferenceEquals(null, pushNotificationAggregator) == true) throw new ArgumentNullException(nameof(pushNotificationAggregator));
            this.pushNotificationAggregator = pushNotificationAggregator;
        }

        public SendTokensResult Send(SubscriptionToken token, NotificationForDelivery notification)
        {
            if (ReferenceEquals(null, token) == true) throw new ArgumentNullException(nameof(token));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));

            if (ReferenceEquals(null, pushNotificationAggregator) == false)
                return pushNotificationAggregator.Queue(token, notification);

            return Send(new List<SubscriptionToken> { token }, notification);
        }

        public SendTokensResult Send(IList<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            if (ReferenceEquals(null, tokens) == true) throw new ArgumentNullException(nameof(tokens));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));
            if (tokens.Count == 0) throw new ArgumentException("Tokens are missing");

            string resource = "push?api_key=" + serverKey;

            log.Debug(() => $"[PushyBase] sending '{tokens.Count}' PN for notification '{notification.Id}' with body '{notification.NotificationPayload.Body}'");
            var sendPushNotificationTokensResult = new SendTokensResult(new List<SubscriptionToken>());

            var tokensAsStrings = tokens.Select(x => x.ToString()).ToList();
            var payload = notification.NotificationPayload;
            var pushySendNotificationModel = new PushySendNotificationModel(payload.Title, payload.Body, payload.Sound, payload.Badge);
            var model = new PushySendModel(tokensAsStrings, pushySendNotificationModel, notification.NotificationData, notification.ExpiresAt, notification.ContentAvailable);
            var request = CreateRestRequest(resource, Method.POST).AddJsonBody(model);
            var response = restClient.Execute<PushyResponseModel>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK || response.HasDataFailure())
            {
                response.LogPushyError(() => $"[PushyBase] failure: status code '{response.StatusCode}'. PN body '{notification.NotificationPayload.Body}'");
                return SendTokensResult.Failed;
            }

            log.Info($"[Pushy] success: pn with body {notification.NotificationPayload?.Body} was sent to {tokens.Count} tokens");
            return sendPushNotificationTokensResult;
        }

        public bool SendToTopic(Topic topic, NotificationForDelivery notification)
        {
            if (ReferenceEquals(null, topic) == true) throw new ArgumentNullException(nameof(topic));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));

            string resource = "push?api_key=" + serverKey;

            log.Debug(() => $"[PushyBase] sending PN for notification '{notification.Id}' with body '{notification.NotificationPayload.Body} to topic '{topic}''");
            var payload = notification.NotificationPayload;
            var pushySendNotificationModel = new PushySendNotificationModel(payload.Title, payload.Body, payload.Sound, payload.Badge);
            var model = new PushySendModel(topic, pushySendNotificationModel, notification.NotificationData, notification.ExpiresAt, notification.ContentAvailable);
            IRestRequest request = CreateRestRequest(resource, Method.POST).AddJsonBody(model);
            var response = restClient.Execute<PushyResponseModel>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK || response.HasDataFailure())
            {
                response.LogPushyError(() => $"[PushyBase] failure: status code '{response.StatusCode}'. PN body '{notification.NotificationPayload.Body}'");
                return false;
            }

            log.Info($"[Pushy] success: pn with body {notification.NotificationPayload?.Body} was sent to topic  '{topic}'");
            return true;
        }

        IRestRequest CreateRestRequest(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = serializer;

            return request;
        }
    }
}
