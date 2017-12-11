using System.Collections.Generic;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationDelivery
    {
        bool Send(SubscriptionToken token, NotificationForDelivery notification);
    }
}
