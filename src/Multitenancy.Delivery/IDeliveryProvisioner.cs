using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.Subscriptions;
using System.Collections.Generic;

namespace Multitenancy.Delivery
{
    public interface IDeliveryProvisioner
    {
        IPushNotificationDelivery ResolveDelivery(SubscriptionType subscriptionType, NotificationForDelivery notification);

        IEnumerable<IPushNotificationDelivery> GetDeliveryProviders(string tenant);
    }
}
