using System;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.WS.Multitenancy
{
    public class DeliveryThatSupportsMultipleDeleries : IPushNotificationDelivery
    {
        readonly IPushNotificationDeliveryResolver deliveryResolver;

        public DeliveryThatSupportsMultipleDeleries(IPushNotificationDeliveryResolver deliveryResolver)
        {
            if (ReferenceEquals(null, deliveryResolver)) throw new ArgumentNullException(nameof(deliveryResolver));
            this.deliveryResolver = deliveryResolver;
        }

        public void Send(SubscriptionToken token, NotificationDeliveryModel notification)
        {
            deliveryResolver.Resolve(notification).Send(token, notification);
        }
    }
}
