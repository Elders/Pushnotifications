using System.Collections.Generic;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationBufferedDelivery
    {
        bool Send(IList<SubscriptionToken> tokens, NotificationForDelivery notification);
    }
}
