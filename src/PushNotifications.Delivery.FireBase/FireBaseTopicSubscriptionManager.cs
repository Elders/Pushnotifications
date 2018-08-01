﻿using System;
using System.Linq;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery.FireBase.Logging;
using PushNotifications.Delivery.FireBase.Models;
using RestSharp;
using RestSharp.Serializers;

namespace PushNotifications.Delivery.FireBase
{
    public class FireBaseTopicSubscriptionManager : ITopicSubscriptionManager
    {
        static ILog log = LogProvider.GetLogger(typeof(FireBaseTopicSubscriptionManager));

        readonly string serverKey;

        readonly IRestClient restClient;

        readonly ISerializer serializer;

        public FireBaseTopicSubscriptionManager(IRestClient restClient, ISerializer serializer, string serverKey)
        {
            if (ReferenceEquals(null, restClient) == true) throw new ArgumentNullException(nameof(restClient));
            if (ReferenceEquals(null, serializer) == true) throw new ArgumentNullException(nameof(serializer));
            if (string.IsNullOrEmpty(serverKey) == true) throw new ArgumentNullException(nameof(serverKey));

            this.restClient = restClient;
            this.serializer = serializer;
            this.serverKey = serverKey;
        }

        IRestRequest CreateRestRequest(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", $"key={serverKey}");
            request.JsonSerializer = serializer;

            return request;
        }

        public bool SubscribeToTopic(SubscriptionToken token, Topic topic)
        {
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));
            if (ReferenceEquals(null, token)) throw new ArgumentNullException(nameof(token));

            const string resource = "/iid/v1:batchAdd";

            log.Debug(() => $"[FireBase] subscribing '{token.Token}' to topic: `{topic}'");
            var model = new FireBaseSubscriptionModel(token.Token, topic);
            var request = CreateRestRequest(resource, Method.POST).AddJsonBody(model);

            var result = restClient.Execute<FireBaseResponseModel>(request);

            if (result.StatusCode != System.Net.HttpStatusCode.OK || result.HasDataFailure())
            {
                result.LogFireBaseError(() => $"[FireBase] failure: status code '{result.StatusCode}'. subscription token: '{token.Token}' from topic: {topic}'");
                return false;
            }

            log.Info($"[FireBase] success: subscribed `{token.Token}` to topic: `{topic}`");
            return true;
        }

        public bool UnsubscribeFromTopic(SubscriptionToken token, Topic topic)
        {
            if (ReferenceEquals(null, token)) throw new ArgumentNullException(nameof(token));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            const string resource = "/iid/v1:batchRemove";

            log.Debug(() => $"[FireBase] unsubscribing '{token.Token}' from topic: `{topic}'");
            var model = new FireBaseSubscriptionModel(token.Token, topic);
            var request = CreateRestRequest(resource, Method.POST).AddJsonBody(model);

            var result = restClient.Execute<FireBaseResponseModel>(request);

            if (result.StatusCode != System.Net.HttpStatusCode.OK || result.HasDataFailure())
            {
                result.LogFireBaseError(() => $"[FireBase] failure: status code '{result.StatusCode}'. subscription token: '{token.Token}' from topic: {topic}'");
                return false;
            }

            log.Info($"[FireBase] success: unsubscribed `{token.Token}` from topic: `{topic}`");
            return true;
        }
    }

    public static class FireBaseExtensions
    {
        static ILog log = LogProvider.GetLogger(typeof(FireBaseTopicSubscriptionManager));

        public static bool HasDataFailure(this IRestResponse<FireBaseResponseModel> response)
        {
            return (response.Data is null == false) && response.Data.Failure == true;
        }

        public static string GetDataErrors(this IRestResponse<FireBaseResponseModel> response)
        {
            if (response.HasDataFailure())
            {
                string dataErrors = $"DataErrors:{Environment.NewLine}" + string.Join(",", response.Data.Results.Select(x => x.Error));
                return dataErrors;
            }

            return string.Empty;
        }

        public static void LogFireBaseError(this IRestResponse<FireBaseResponseModel> response, Func<string> contextualMessage)
        {
            if (response.ErrorException is null)
            {
                log.Error(() =>
                {
                    string dataErrors = response.GetDataErrors();
                    return $"{contextualMessage()}{Environment.NewLine}{dataErrors}{Environment.NewLine}{response.ErrorMessage}";
                });
            }
            else
            {
                string dataErrors = response.GetDataErrors();
                string errorMessage = $"{contextualMessage()}{Environment.NewLine}{dataErrors}{Environment.NewLine}{response.ErrorMessage}";
                log.ErrorException(errorMessage, response.ErrorException);
            }
        }
    }
}
