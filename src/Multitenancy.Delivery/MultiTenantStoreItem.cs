using System;
using Elders.Cronus;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.Subscriptions;

namespace Multitenancy.Delivery
{
    public class MultiTenantStoreItem : ValueObject<MultiTenantStoreItem>
    {
        public MultiTenantStoreItem(string tenant, SubscriptionType subscriptionType, IPushNotificationDelivery delivery)
        {
            if (string.IsNullOrEmpty(tenant) == true) throw new ArgumentNullException(nameof(tenant));
            if (ReferenceEquals(null, subscriptionType) == true) throw new ArgumentNullException(nameof(subscriptionType));
            if (ReferenceEquals(null, delivery) == true) throw new ArgumentNullException(nameof(delivery));

            Tenant = tenant;
            SubscriptionType = subscriptionType;
            Delivery = delivery;
        }

        public string Tenant { get; private set; }

        public SubscriptionType SubscriptionType { get; private set; }

        public IPushNotificationDelivery Delivery { get; private set; }
    }
}
