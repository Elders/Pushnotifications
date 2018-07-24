using System;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery.Pushy.Logging;
using PushNotifications.Delivery.Pushy.Models;
using RestSharp;
using RestSharp.Serializers;

namespace PushNotifications.Delivery.Pushy
{
    public class PushyTopicSubscriptionManager : ITopicSubscriptionManager
    {
        static ILog log = LogProvider.GetLogger(typeof(PushyTopicSubscriptionManager));

        readonly string serverKey;

        readonly IRestClient restClient;

        readonly ISerializer serializer;

        public PushyTopicSubscriptionManager(IRestClient restClient, ISerializer serializer, string serverKey)
        {
            if (ReferenceEquals(null, restClient) == true) throw new ArgumentNullException(nameof(restClient));
            if (ReferenceEquals(null, serializer) == true) throw new ArgumentNullException(nameof(serializer));
            if (string.IsNullOrEmpty(serverKey) == true) throw new ArgumentNullException(nameof(serverKey));

            this.restClient = restClient;
            this.serializer = serializer;
            this.serverKey = serverKey;
        }

        public bool SubscribeToTopic(SubscriptionToken token, Topic topic)
        {
            if (ReferenceEquals(null, topic) == true) throw new ArgumentNullException(nameof(topic));
            if (ReferenceEquals(null, token) == true) throw new ArgumentNullException(nameof(token));

            string resource = "devices/subscribe?api_key=" + serverKey;

            log.Debug(() => $"[PushyBase] subscribing token '{token.Token}' to topic '{topic}''");

            var model = new PushyTopicSubscriptionModel(token, topic);
            IRestRequest request = CreateRestRequest(resource, Method.POST).AddJsonBody(model);
            var result = restClient.Execute<PushyResponseModel>(request);

            if (result.StatusCode != System.Net.HttpStatusCode.OK || result.Data.Success == false)
            {
                log.Error(() => $"[PushyBase] failure: status code '{result.StatusCode}' and error '{result.Data.Error}'. token '{token.Token}' and topic '{topic}'");
                return false;
            }

            log.Info($"[Pushy] success: subscribed token {token.Token} to topic  '{topic}'");
            return true;
        }

        public bool UnsubscribeFromTopic(SubscriptionToken token, Topic topic)
        {
            if (ReferenceEquals(null, topic) == true) throw new ArgumentNullException(nameof(topic));
            if (ReferenceEquals(null, token) == true) throw new ArgumentNullException(nameof(token));

            string resource = "devices/unsubscribe?api_key=" + serverKey;

            log.Debug(() => $"[PushyBase] unsubscribing token '{token.Token}' from topic '{topic}''");

            var model = new PushyTopicSubscriptionModel(token, topic);
            IRestRequest request = CreateRestRequest(resource, Method.POST).AddJsonBody(model);
            var result = restClient.Execute<PushyResponseModel>(request);

            if (result.StatusCode != System.Net.HttpStatusCode.OK || result.Data.Success == false)
            {
                log.Error(() => $"[PushyBase] failure: status code '{result.StatusCode}' and error '{result.Data.Error}'. token '{token.Token}' and topic '{topic}'");
                return false;
            }

            log.Info($"[Pushy] success: unsubscribed token {token.Token} from topic  '{topic}'");
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
