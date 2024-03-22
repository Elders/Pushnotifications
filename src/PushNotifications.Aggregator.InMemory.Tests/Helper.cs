using System;
using System.Collections.Generic;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Subscriptions;

namespace PushNotifications.Aggregator.InMemory.Tests
{
    public static class Helper
    {
        public static void Send(IPushNotificationDelivery theDelivery, int count, NotificationForDelivery notification)
        {
            for (int i = 0; i < count; i++)
            {
                var token = new SubscriptionToken(Guid.NewGuid().ToString(), SubscriptionType.FireBase);
                theDelivery.SendAsync(new List<SubscriptionToken>() { token }, notification);
            }
        }
    }
}
