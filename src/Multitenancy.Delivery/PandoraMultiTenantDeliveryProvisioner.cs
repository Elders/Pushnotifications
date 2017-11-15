using System;
using System.Collections.Generic;
using System.Linq;
using Elders.Pandora;
using Multitenancy.Delivery.Serialization;
using PushNotifications.Contracts.FireBaseSubscriptions;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.PushySubscriptions;
using PushNotifications.Delivery.Bulk;
using PushNotifications.Delivery.FireBase;
using PushNotifications.Delivery.Pushy;

namespace Multitenancy.Delivery
{
    public class PandoraMultiTenantDeliveryProvisioner : IDeliveryProvisioner
    {
        readonly ISet<MultiTenantStoreItem> store;

        readonly Pandora pandora;

        public PandoraMultiTenantDeliveryProvisioner(Pandora pandora)
        {
            if (ReferenceEquals(null, pandora) == true) throw new ArgumentNullException(nameof(pandora));

            this.store = new HashSet<MultiTenantStoreItem>();
            this.pandora = pandora;
            Initialize();
        }

        public IPushNotificationDelivery ResolveDelivery(NotificationDeliveryModel notification)
        {
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));

            var tenant = notification.Id.Tenant;
            var notificationType = notification.GetType();
            var storeItem = store.SingleOrDefault(x => x.Tenant == tenant && x.Type == notificationType);

            if (ReferenceEquals(null, storeItem) == true) throw new NotSupportedException($"There is no registered delivery for type {notificationType.Name} and tenant {tenant}");
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
            var timeSpanBeforeFlush = TimeSpan.FromSeconds(settings.TimeSpanBeforeFlushInSeconds);
            var recipientsCountBeforeFlush = settings.RecipientsCountBeforeFlush;

            var pushyRestClient = new RestSharp.RestClient(baseUrl);
            var pushyDelivery = new PushyDelivery(pushyRestClient, NewtonsoftJsonSerializer.Default(), settings.ServerKey);
            var pushyBulkDelivery = new BulkDelivery<PushyDelivery>(pushyDelivery, timeSpanBeforeFlush, recipientsCountBeforeFlush);

            var type = typeof(PushyNotificationDelivery);
            store.Add(new MultiTenantStoreItem(settings.Tenant, type, pushyBulkDelivery));
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
