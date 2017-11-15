using System;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace Multitenancy.Delivery
{
    public class MultiTenantDelivery : IPushNotificationDelivery
    {
        readonly IDeliveryProvisioner multiTenantDeliveryProvisioner;

        public MultiTenantDelivery(IDeliveryProvisioner multiTenantDeliveryProvisioner)
        {
            if (ReferenceEquals(null, multiTenantDeliveryProvisioner)) throw new ArgumentNullException(nameof(multiTenantDeliveryProvisioner));

            this.multiTenantDeliveryProvisioner = multiTenantDeliveryProvisioner;
        }

        public void Send(SubscriptionToken token, NotificationDeliveryModel notification)
        {
            if (ReferenceEquals(null, token) == true) throw new ArgumentNullException(nameof(token));
            if (ReferenceEquals(null, notification) == true) throw new ArgumentNullException(nameof(notification));

            var delivery = multiTenantDeliveryProvisioner.ResolveDelivery(notification);

            delivery.Send(token, notification);
        }
    }
}
