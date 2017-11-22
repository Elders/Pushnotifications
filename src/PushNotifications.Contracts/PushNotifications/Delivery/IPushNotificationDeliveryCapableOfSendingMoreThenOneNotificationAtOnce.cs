using System.Collections.Generic;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationDeliveryCapableOfSendingMoreThenOneNotificationAtOnce
    {
        bool Send(IList<SubscriptionToken> tokens, NotificationForDelivery notification);
    }
}
