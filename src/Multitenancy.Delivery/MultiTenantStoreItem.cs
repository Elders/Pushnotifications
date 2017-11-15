using System;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace Multitenancy.Delivery
{
    public class MultiTenantStoreItem : ValueObject<MultiTenantStoreItem>
    {
        public MultiTenantStoreItem(string tenant, Type type, IPushNotificationDelivery delivery)
        {
            Tenant = tenant;
            Type = type;
            Delivery = delivery;
        }

        public string Tenant { get; private set; }

        public Type Type { get; private set; }

        public IPushNotificationDelivery Delivery { get; private set; }
    }
}
