using System;
using PushNotifications.Api.Client.Models;
using PushNotifications.Converters;
using RestSharp;
using System.Linq;
using Newtonsoft.Json;
using PushNotifications.Contracts;

namespace PushNotifications.Api.Client
{
    public class PushNotificationsRestClient
    {
        readonly RestSharpIdentityModelClient restSharpIdentityModelClient;

        public PushNotificationsRestClient(Uri authority, string authorizationEndpointRelativePath, string clientId, string clientSecret, string scope, Uri apiAddress)
        {
            if (ReferenceEquals(authority, null) == true) throw new ArgumentNullException(nameof(authority));
            if (string.IsNullOrEmpty(authorizationEndpointRelativePath) == true) throw new ArgumentNullException(nameof(authorizationEndpointRelativePath));
            if (string.IsNullOrEmpty(clientId) == true) throw new ArgumentNullException(nameof(clientId));
            if (string.IsNullOrEmpty(clientSecret) == true) throw new ArgumentNullException(nameof(clientSecret));
            if (ReferenceEquals(apiAddress, null) == true) throw new ArgumentNullException(nameof(apiAddress));

            var authenticatorClientOptions = Authenticator.Options.UseClientCredentials(authority, clientId, clientSecret, scope, authorizationEndpointRelativePath);
            var authenticator = new Authenticator(authenticatorClientOptions);
            var clientCredentialsAuthenticator = authenticator.GetClientCredentialsAuthenticatorAsync().Result;

            var serializerSettings = SerializerFactory.DefaultSettings();
            var converters = typeof(PushNotificationsConvertersAssembly).Assembly.GetTypes()
                .Where(x => typeof(JsonConverter).IsAssignableFrom(x) && x.IsAbstract == false);

            foreach (var item in converters)
            {
                serializerSettings.Converters.Add(Activator.CreateInstance(item) as JsonConverter);
            }

            var localSerializer = new NewtonsoftJsonSerializer(Newtonsoft.Json.JsonSerializer.Create(serializerSettings));
            var restSharpIdentityModelClientOptions = new RestSharpIdentityModelClient.Options(apiAddress, localSerializer);

            restSharpIdentityModelClient = new RestSharpIdentityModelClient(restSharpIdentityModelClientOptions, clientCredentialsAuthenticator);
        }

        public IRestResponse SendPushNotification(PushNotificationSendModel pushNotification, Authenticator authenticator = null)
        {
            if (ReferenceEquals(null, pushNotification) == true) throw new ArgumentNullException(nameof(pushNotification));

            const string resource = "PushNotifications/Send";

            return restSharpIdentityModelClient.Execute<ResponseResult>(resource, Method.POST, pushNotification, authenticator);
        }

        public IRestResponse SubscribeForFireBase(SubscriptionForFireBase subscription, Authenticator authenticator = null)
        {
            if (ReferenceEquals(null, subscription) == true) throw new ArgumentNullException(nameof(subscription));

            const string resource = "Subscriptions/FireBaseSubscription/Subscribe";

            return restSharpIdentityModelClient.Execute<ResponseResult>(resource, Method.POST, subscription, authenticator);
        }

        public IRestResponse SubscribeForPushy(SubscriptionForPushy subscription, Authenticator authenticator = null)
        {
            if (ReferenceEquals(null, subscription) == true) throw new ArgumentNullException(nameof(subscription));

            const string resource = "Subscriptions/PushySubscription/Subscribe";

            return restSharpIdentityModelClient.Execute<ResponseResult>(resource, Method.POST, subscription, authenticator);
        }

        public IRestResponse UnSubscribeForFireBase(SubscriptionForFireBase subscription, Authenticator authenticator = null)
        {
            if (ReferenceEquals(null, subscription) == true) throw new ArgumentNullException(nameof(subscription));

            const string resource = "Subscriptions/FireBaseSubscription/UnSubscribe";

            return restSharpIdentityModelClient.Execute<ResponseResult>(resource, Method.POST, subscription, authenticator);
        }

        public IRestResponse UnSubscribeForPushy(SubscriptionForPushy subscription, Authenticator authenticator = null)
        {
            if (ReferenceEquals(null, subscription) == true) throw new ArgumentNullException(nameof(subscription));

            const string resource = "Subscriptions/PushySubscription/UnSubscribe";

            return restSharpIdentityModelClient.Execute<ResponseResult>(resource, Method.POST, subscription, authenticator);
        }
    }
}
