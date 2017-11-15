using System.Collections.Generic;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationBulkDelivery
    {
        bool Send(IList<SubscriptionToken> tokens, NotificationDeliveryModel notification);
    }
}
