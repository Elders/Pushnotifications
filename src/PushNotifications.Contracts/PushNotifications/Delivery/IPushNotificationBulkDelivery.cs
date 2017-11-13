using System.Collections.Generic;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationBulkDelivery
    {
        void Send(IList<SubscriptionToken> tokens, NotificationDeliveryModel notification);
    }
}
