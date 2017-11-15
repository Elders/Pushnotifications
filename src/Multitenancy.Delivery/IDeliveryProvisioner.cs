using System;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace Multitenancy.Delivery
{
    public interface IDeliveryProvisioner
    {
        IPushNotificationDelivery ResolveDelivery(NotificationDeliveryModel notification);
    }
}
