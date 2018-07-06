using System;
using System.Collections.Generic;
using System.Linq;
using Elders.Pandora;
using Multitenancy.Delivery.Serialization;
using PushNotifications.Aggregator.InMemory;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Delivery.FireBase;
using PushNotifications.Delivery.Pushy;

namespace Multitenancy.Delivery
{
    public class PandoraMultiTenantDeliveryProvisioner : IDeliveryProvisioner, ITopicSubscriptionProvisioner
    {
        readonly ISet<MultiTenantStoreItem> _store;

        readonly ISet<MultiTenantStoreSubscriptionItem> _subscriptionStore;

        readonly Pandora pandora;

        public PandoraMultiTenantDeliveryProvisioner(Pandora pandora)
        {
            if (ReferenceEquals(null, pandora) == true) throw new ArgumentNullException(nameof(pandora));

            _store = new HashSet<MultiTenantStoreItem>();
            _subscriptionStore = new HashSet<MultiTenantStoreSubscriptionItem>();
            this.pandora = pandora;
            Initialize();
        }

        public IPushNotificationDelivery ResolveDelivery(SubscriptionType subscriptionType, NotificationForDelivery notification)
        {
            if (ReferenceEquals(null, subscriptionType) == true) throw new ArgumentNullException(nameof(subscriptionType));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));

            var tenant = notification.Id.Tenant;
            var storeItem = _store.SingleOrDefault(x => x.Tenant == tenant && x.SubscriptionType == subscriptionType);

            if (ReferenceEquals(null, storeItem) == true) throw new NotSupportedException($"There is no registered delivery for type '{subscriptionType}' and tenant '{tenant}'");
            return storeItem.Delivery;
        }

        public ITopicSubscriptionManager ResolveTopicSubscriptionManager(SubscriptionType subscriptionType, string tenant)
        {
            if (ReferenceEquals(null, subscriptionType) == true) throw new ArgumentNullException(nameof(subscriptionType));
            if (string.IsNullOrEmpty(tenant)) throw new ArgumentNullException(nameof(tenant));

            MultiTenantStoreSubscriptionItem storeItem = _subscriptionStore.SingleOrDefault(x => x.Tenant == tenant && x.SubscriptionType == subscriptionType);

            if (ReferenceEquals(null, storeItem) == true) throw new NotSupportedException($"There is no registered delivery for type '{subscriptionType}' and tenant '{tenant}'");
            return storeItem.TopicSubscriptionManager;
        }

        void Initialize()
        {
            var firebaseSettings = pandora.Get<List<FireBaseSettings>>("delivery_firebase_settings");
            var firebaseUrl = pandora.Get("delivery_firebase_baseurl");
            var firebaseSubscriptionsUrl = pandora.Get("subscriptions_firebase_baseurl");

            var pushyUrl = pandora.Get("delivery_pushy_baseurl");
            var pushySettings = pandora.Get<List<PushySettings>>("delivery_pushy_settings");

            foreach (var fb in firebaseSettings)
            {
                RegisterFireBaseDelivery(firebaseUrl, fb);
            }

            foreach (var fb in firebaseSettings)
            {
                RegisterFireBaseSubscriptionManager(firebaseUrl, fb);
            }

            foreach (var pushy in pushySettings)
            {
                RegisterPushyDelivery(pushyUrl, pushy);
                RegisterPushySubscriptionManager(pushyUrl, pushy);
            }
        }

        void RegisterFireBaseDelivery(string baseUrl, FireBaseSettings settings)
        {
            if (string.IsNullOrEmpty(baseUrl) == true) throw new ArgumentNullException(nameof(baseUrl));
            if (settings.RecipientsCountBeforeFlush >= 1000) throw new ArgumentException($"FireBase limits the number of tokens to 1000. Use lower number for '{nameof(settings.RecipientsCountBeforeFlush)}'");

            var timeSpanBeforeFlush = TimeSpan.FromSeconds(settings.TimeSpanBeforeFlushInSeconds);
            var recipientsCountBeforeFlush = settings.RecipientsCountBeforeFlush;

            var fireBaseRestClient = new RestSharp.RestClient(baseUrl);
            var fireBaseDelivery = new FireBaseDelivery(fireBaseRestClient, NewtonsoftJsonSerializer.Default(), settings.ServerKey);
            fireBaseDelivery.UseAggregator(new InMemoryPushNotificationAggregator(fireBaseDelivery.Send, timeSpanBeforeFlush, recipientsCountBeforeFlush));

            _store.Add(new MultiTenantStoreItem(settings.Tenant, SubscriptionType.FireBase, fireBaseDelivery));
        }

        void RegisterFireBaseSubscriptionManager(string baseUrl, FireBaseSettings settings)
        {
            if (string.IsNullOrEmpty(baseUrl) == true) throw new ArgumentNullException(nameof(baseUrl));

            var fireBaseRestClient = new RestSharp.RestClient(baseUrl);
            var firebaseTopicSubscriptionManager = new FireBaseTopicSubscriptionManager(fireBaseRestClient, NewtonsoftJsonSerializer.Default(), settings.ServerKey);

            _subscriptionStore.Add(new MultiTenantStoreSubscriptionItem(settings.Tenant, SubscriptionType.FireBase, firebaseTopicSubscriptionManager));
        }

        void RegisterPushySubscriptionManager(string baseUrl, PushySettings settings)
        {
            if (string.IsNullOrEmpty(baseUrl) == true) throw new ArgumentNullException(nameof(baseUrl));

            var pushyRestClient = new RestSharp.RestClient(baseUrl);
            var pushyTopicSubscriptionManager = new PushyTopicSubscriptionManager(pushyRestClient, NewtonsoftJsonSerializer.Default(), settings.ServerKey);

            _subscriptionStore.Add(new MultiTenantStoreSubscriptionItem(settings.Tenant, SubscriptionType.Pushy, pushyTopicSubscriptionManager));
        }

        void RegisterPushyDelivery(string baseUrl, PushySettings settings)
        {
            if (string.IsNullOrEmpty(baseUrl) == true) throw new ArgumentNullException(nameof(baseUrl));

            var timeSpanBeforeFlush = TimeSpan.FromSeconds(settings.TimeSpanBeforeFlushInSeconds);
            var recipientsCountBeforeFlush = settings.RecipientsCountBeforeFlush;

            var pushyRestClient = new RestSharp.RestClient(baseUrl);
            var pushyDelivery = new PushyDelivery(pushyRestClient, NewtonsoftJsonSerializer.Default(), settings.ServerKey);
            pushyDelivery.UseAggregator(new InMemoryPushNotificationAggregator(pushyDelivery.Send, timeSpanBeforeFlush, recipientsCountBeforeFlush));

            _store.Add(new MultiTenantStoreItem(settings.Tenant, SubscriptionType.Pushy, pushyDelivery));
        }

        class PushySettings
        {
            public long TimeSpanBeforeFlushInSeconds { get; set; }

            public int RecipientsCountBeforeFlush { get; set; }

            public string ServerKey { get; set; }

            public string Tenant { get; set; }
        }

        class FireBaseSettings
        {
            public long TimeSpanBeforeFlushInSeconds { get; set; }

            public int RecipientsCountBeforeFlush { get; set; }

            public string ServerKey { get; set; }

            public string Tenant { get; set; }
        }
    }
}
