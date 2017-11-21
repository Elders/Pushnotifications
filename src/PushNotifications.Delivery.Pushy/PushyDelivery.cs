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
    public class PushyDelivery : IPushNotificationDelivery, IPushNotificationBufferedDelivery
    {
        static ILog log = LogProvider.GetLogger(typeof(PushyDelivery));

        readonly string serverKey;

        readonly IRestClient restClient;

        readonly ISerializer serializer;

        public PushyDelivery(IRestClient restClient, ISerializer serializer, string serverKey)
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

            string resource = "push?api_key=" + serverKey;

            var tokensAsStrings = tokens.Select(x => x.ToString()).ToList();
            var payload = notification.NotificationPayload;
            var pushySendNotificationModel = new PushySendNotificationModel(payload.Title, payload.Body, payload.Sound, payload.Badge);
            var pushySendDataModel = new PushySendDataModel(payload.Title, payload.Body, payload.Sound, payload.Badge.ToString());
            var model = new PushySendModel(tokensAsStrings, pushySendNotificationModel, pushySendDataModel, notification.ExpiresAt, notification.ContentAvailable);
            var request = CreateRestRequest(resource, Method.POST).AddJsonBody(model);
            var result = restClient.Execute<PushyResponseModel>(request);

            if (result.StatusCode != System.Net.HttpStatusCode.OK || result.Data.Success == false)
            {
                log.Error(() => $"[PushyBase] failure: status code '{result.StatusCode}' and error '{result.Data.Error}'. PN body '{notification.NotificationPayload.Body}'");
                return false;
            }
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
