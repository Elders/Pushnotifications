﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elders.Cronus;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery.FireBase.Logging;
using PushNotifications.Delivery.FireBase.Models;
using RestSharp;
using RestSharp.Serializers;

namespace PushNotifications.Delivery.FireBase
{
    public class FireBaseDelivery : IPushNotificationDelivery
    {
        static ILog log = LogProvider.GetLogger(typeof(FireBaseDelivery));

        readonly string serverKey;

        readonly IRestClient restClient;

        readonly ISerializer serializer;

        IPushNotificationAggregator pushNotificationAggregator;

        public FireBaseDelivery(IRestClient restClient, ISerializer serializer, string serverKey)
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

            const string resource = "fcm/send";

            log.Debug(() => $"[FireBase] sending '{tokens.Count}' PN for notification '{notification.Id}' with body '{notification.NotificationPayload.Body}'");

            List<string> tokensAsStrings = tokens.Select(x => x.ToString()).ToList();
            NotificationPayload payload = notification.NotificationPayload;
            Dictionary<string, object> data = notification.NotificationData;
            string badge = payload.Badge > 0 ? payload.Badge.ToString() : "1";
            var fireBaseSendNotificationModel = new FireBaseSendNotificationModel(payload.Title, payload.Body, payload.Sound, badge);
            var model = new FireBaseSendModel(tokensAsStrings, fireBaseSendNotificationModel, data, notification.ExpiresAt);
            IRestRequest request = CreateRestRequest(resource, Method.POST).AddJsonBody(model);
            IRestResponse<FireBaseResponseModel> result = restClient.Execute<FireBaseResponseModel>(request);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var sendPushNotificationResult = GetNotRegisteredTokens(tokens, result);
                log.Info($"[FireBase] success: PN with body {notification.NotificationPayload?.Body} was sent to {tokens.Count} tokens");
                return sendPushNotificationResult;
            }
            else
            {
                var error = string.Join(",", result.Data.Results.Select(x => x.Error));
                log.Error(() => $"[FireBase] failure: status code '{result.StatusCode}' and error '{error}'. PN body '{notification.NotificationPayload.Body}'");

                return SendTokensResult.Success;
            }
        }

        private static SendTokensResult GetNotRegisteredTokens(IList<SubscriptionToken> tokens, IRestResponse<FireBaseResponseModel> result)
        {
            var sendPushNotificationResult = new List<SubscriptionToken>();

            if (result.Data.Failure == true)
            {
                for (int i = 0; i < result.Data.Results.Count; i++)
                {
                    var token = tokens[i];
                    if (string.Equals(result.Data.Results[i].Error, "NotRegistered", StringComparison.OrdinalIgnoreCase))
                    {
                        log.Info($"[FireBase] the token: '{token}' is not registered and will be removed from the subscriber");
                        sendPushNotificationResult.Add(token);
                    }
                }
                return new SendTokensResult(sendPushNotificationResult);
            }
            return SendTokensResult.Success;
        }

        public bool SendToTopic(Topic topic, NotificationForDelivery notification)
        {
            if (ReferenceEquals(null, topic) == true) throw new ArgumentNullException(nameof(topic));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));

            const string resource = "fcm/send";

            log.Debug(() => $"[FireBase] sending PN to topic: {topic} for notification '{notification.Id}' with body '{notification.NotificationPayload.Body}'");
            var payload = notification.NotificationPayload;
            var data = notification.NotificationData;
            string badge = payload.Badge > 0 ? payload.Badge.ToString() : "1";

            var fireBaseSendNotificationModel = new FireBaseSendNotificationModel(payload.Title, payload.Body, payload.Sound, badge);
            var model = new FireBaseTopicSendModel(topic, fireBaseSendNotificationModel, data, notification.ExpiresAt);

            var request = CreateRestRequest(resource, Method.POST).AddJsonBody(model);
            var result = restClient.Execute<FireBaseResponseModel>(request);

            if (result.StatusCode != System.Net.HttpStatusCode.OK || result.Data.Failure == true)
            {
                var error = string.Join(",", result.Data.Results.Select(x => x.Error));
                log.Error(() => $"[FireBase] failure: status code '{result.StatusCode}' and error '{error}'. PN body '{notification.NotificationPayload.Body}'");
                return false;
            }

            log.Info($"[FireBase] success: PN with body {notification.NotificationPayload?.Body} was sent to {topic} topic");
            return true;
        }

        IRestRequest CreateRestRequest(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", $"key={serverKey}");
            request.JsonSerializer = serializer;

            return request;
        }

    }
}
