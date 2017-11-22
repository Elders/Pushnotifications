﻿using System;
using System.Collections.Generic;
using System.Linq;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery.FireBase.Logging;
using PushNotifications.Delivery.FireBase.Models;
using RestSharp;
using RestSharp.Serializers;

namespace PushNotifications.Delivery.FireBase
{
    public class FireBaseDelivery : IPushNotificationDelivery, IPushNotificationDeliveryCapableOfSendingMoreThenOneNotificationAtOnce
    {
        static ILog log = LogProvider.GetLogger(typeof(FireBaseDelivery));

        readonly string serverKey;

        readonly IRestClient restClient;

        readonly ISerializer serializer;

        public FireBaseDelivery(IRestClient restClient, ISerializer serializer, string serverKey)
        {
            if (ReferenceEquals(null, restClient) == true) throw new ArgumentNullException(nameof(restClient));
            if (ReferenceEquals(null, serializer) == true) throw new ArgumentNullException(nameof(serializer));
            if (string.IsNullOrEmpty(serverKey) == true) throw new ArgumentNullException(nameof(serverKey));

            this.restClient = restClient;
            this.serializer = serializer;
            this.serverKey = serverKey;
        }

        public bool Send(SubscriptionToken token, NotificationForDelivery notification)
        {
            return Send(new List<SubscriptionToken> { token }, notification);
        }

        public bool Send(IList<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            if (ReferenceEquals(null, tokens) == true) throw new ArgumentNullException(nameof(tokens));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));
            if (tokens.Count == 0) throw new ArgumentException("Tokens are missing");

            const string resource = "fcm/send";

            var tokensAsStrings = tokens.Select(x => x.ToString()).ToList();
            var payload = notification.NotificationPayload;
            var fireBaseSendNotificationModel = new FireBaseSendNotificationModel(payload.Title, payload.Body, payload.Sound, payload.Badge.ToString());
            var model = new FireBaseSendModel(tokensAsStrings, fireBaseSendNotificationModel, notification.ExpiresAt, notification.ContentAvailable);
            var request = CreateRestRequest(resource, Method.POST).AddJsonBody(model);
            var result = restClient.Execute<FireBaseResponseModel>(request);

            if (result.StatusCode != System.Net.HttpStatusCode.OK || result.Data.Failure == true)
            {
                var error = string.Join(",", result.Data.Results.Select(x => x.Error));
                log.Error(() => $"[FireBase] failure: status code '{result.StatusCode}' and error '{error}'. PN body '{notification.NotificationPayload.Body}'");
                return false;
            }
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
