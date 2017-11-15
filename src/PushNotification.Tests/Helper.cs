using System;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Delivery.Bulk;

namespace PushNotification.Tests
{
    public class Helper
    {
        public void Send(InMemoryBufferedDelivery<IPushNotificationBulkDelivery> theDelivery, int count, NotificationDeliveryModel notification)
        {
            for (int i = 0; i < count; i++)
            {
                var token = new SubscriptionToken(Guid.NewGuid().ToString());
                theDelivery.Send(token, notification);
            }
        }
    }
}
