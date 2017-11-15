using System;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace Multitenancy.Delivery
{
    public interface IMultiTenantDeliveryProvisioner
    {
        IPushNotificationDelivery GetDelivery(string tenant, Type notificationType);
    }
}
