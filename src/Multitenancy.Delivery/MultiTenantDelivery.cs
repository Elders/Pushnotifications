using System;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace Multitenancy.Delivery
{
    public class MultiTenantDelivery : IPushNotificationDelivery
    {
        readonly IMultiTenantDeliveryProvisioner multiTenantDeliveryProvisioner;

        public MultiTenantDelivery(IMultiTenantDeliveryProvisioner multiTenantDeliveryProvisioner)
        {
            if (ReferenceEquals(null, multiTenantDeliveryProvisioner)) throw new ArgumentNullException(nameof(multiTenantDeliveryProvisioner));

            this.multiTenantDeliveryProvisioner = multiTenantDeliveryProvisioner;
        }

        public void Send(SubscriptionToken token, NotificationDeliveryModel notification)
        {
            var tenant = notification.Id.Tenant;
            var notificationType = notification.GetType();
            var delivery = multiTenantDeliveryProvisioner.GetDelivery(tenant, notificationType);

            delivery.Send(token, notification);
        }
    }
}
