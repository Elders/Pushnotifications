using System;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts;
using PushNotifications.Delivery.Buffered;

namespace PushNotification.Tests
{
    public static class Helper
    {
        public static void Send(InMemoryBufferedDelivery<IPushNotificationBufferedDelivery> theDelivery, int count, NotificationForDelivery notification)
        {
            for (int i = 0; i < count; i++)
            {
                var token = new SubscriptionToken(Guid.NewGuid().ToString());
                theDelivery.Send(token, notification);
            }
        }
    }
}
