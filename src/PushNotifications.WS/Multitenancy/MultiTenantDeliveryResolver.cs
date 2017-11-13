using System;
using System.Collections.Generic;
using System.Linq;
using Elders.Pandora;
using Multitenancy.TenantResolver;
using PushNotifications.Contracts.FireBaseSubscriptions;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.PushySubscriptions;
using PushNotifications.Delivery.Bulk;
using PushNotifications.Delivery.FireBase;
using PushNotifications.Delivery.Pushy;
using PushNotifications.WS.Serialization;

namespace PushNotifications.WS.Multitenancy
{
    public class MultiTenantDeliveryResolver : IPushNotificationDeliveryResolver
    {
        readonly ISet<MultiTenantStoreItem> store;

        readonly ITenantResolver tenantResolver;

        readonly Pandora pandora;

        public MultiTenantDeliveryResolver(ITenantResolver tenantResolver, Pandora pandora)
        {
            if (ReferenceEquals(null, tenantResolver) == true) throw new ArgumentNullException(nameof(tenantResolver));
            if (ReferenceEquals(null, pandora) == true) throw new ArgumentNullException(nameof(pandora));

            store = new HashSet<MultiTenantStoreItem>();
            this.tenantResolver = tenantResolver;
            this.pandora = pandora;

            Initialize();
        }

        public IPushNotificationDelivery Resolve(NotificationDeliveryModel notification)
        {
            var tenant = tenantResolver.Resolve(notification.Id);
            var type = notification.GetType();
            var storeItem = store.FirstOrDefault(x => x.Tenant == tenant && x.Type == type);

            if (ReferenceEquals(null, storeItem) == true) throw new NotSupportedException($"There is no registered delivery for type {type.Name} and tenant {tenant}");
            return storeItem.Delivery;
        }

        void Initialize()
        {
            var firebaseSettings = pandora.Get<List<FireBaseSettings>>("delivery_firebase_settings");
            var pushySettings = pandora.Get<List<PushySettings>>("delivery_pushy_settings");

            foreach (var fb in firebaseSettings)
            {
                RegisterFireBaseDelivery(fb);
            }

            foreach (var pushy in pushySettings)
            {
                RegisterPushyDelivery(pushy);
            }
        }

        void RegisterFireBaseDelivery(FireBaseSettings settings)
        {
            var baseUrl = "https://fcm.googleapis.com/";
            //var serverKey = "AAAAqg7V420:APA91bEggfsid7oJGnlravJ0gwCJ8ZMthEfWTfecHMOQjYVFdToIXxLXQj0oomBeVDNYCFZQ_sfbqASpsGcqOkJdKASpxCxGYHvof3ngENX_iSD_bl65PriDIAESPhhvqNeBZqw0wb4Z";
            var timeSpanBeforeFlush = TimeSpan.FromSeconds(settings.TimeSpanBeforeFlushInSeconds);
            var recipientsCountBeforeFlush = settings.RecipientsCountBeforeFlush;

            var fireBaseRestClient = new RestSharp.RestClient(baseUrl);
            var fireBaseDelivery = new FireBaseDelivery(fireBaseRestClient, NewtonsoftJsonSerializer.Default(), settings.ServerKey);
            var fireBaseBulkDelivery = new BulkDelivery<FireBaseDelivery>(fireBaseDelivery, timeSpanBeforeFlush, recipientsCountBeforeFlush);

            var type = typeof(FireBaseNotificationDelivery);
            store.Add(new MultiTenantStoreItem(settings.Tenant, type, fireBaseBulkDelivery));
        }

        void RegisterPushyDelivery(PushySettings settings)
        {
            var baseUrl = "https://api.pushy.me/";
            //var serverKey = "cd8100e3f367777f2dd999c5d97c5cc30d099fcf4da073749232140e365e09f9";
            var timeSpanBeforeFlush = TimeSpan.FromSeconds(settings.TimeSpanBeforeFlushInSeconds);
            var recipientsCountBeforeFlush = settings.RecipientsCountBeforeFlush;

            var pushyRestClient = new RestSharp.RestClient(baseUrl);
            var pushyDelivery = new PushyDelivery(pushyRestClient, NewtonsoftJsonSerializer.Default(), settings.ServerKey);
            var pushyBulkDelivery = new BulkDelivery<PushyDelivery>(pushyDelivery, timeSpanBeforeFlush, recipientsCountBeforeFlush);

            var type = typeof(PushyNotificationDelivery);
            store.Add(new MultiTenantStoreItem(settings.Tenant, type, pushyBulkDelivery));
        }
    }

    public class FireBaseSettings
    {
        public long TimeSpanBeforeFlushInSeconds { get; set; }

        public int RecipientsCountBeforeFlush { get; set; }

        public string ServerKey { get; set; }

        public string Tenant { get; set; }
    }

    public class PushySettings
    {
        public long TimeSpanBeforeFlushInSeconds { get; set; }

        public int RecipientsCountBeforeFlush { get; set; }

        public string ServerKey { get; set; }

        public string Tenant { get; set; }
    }
}
