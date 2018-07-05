﻿using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.Subscriptions;

namespace Multitenancy.Delivery
{
    public interface IDeliveryProvisioner
    {
        IPushNotificationDelivery ResolveDelivery(SubscriptionType subscriptionType, NotificationForDelivery notification);

        IPushNotificationDelivery ResolveDelivery(SubscriptionType subscriptionType, string tenant);
    }
}
