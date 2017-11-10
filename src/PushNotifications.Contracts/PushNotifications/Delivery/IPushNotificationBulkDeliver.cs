using System;
using System.Collections.Generic;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationBulkDeliver
    {
        void Send(IList<SubscriptionToken> tokens, NotificationDelivery notification);
    }
}
