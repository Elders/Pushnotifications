using System.Collections.Generic;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotification.Tests
{
    public class TestDelivery : IPushNotificationDeliver, IPushNotificationBulkDeliver
    {
        readonly List<KeyValuePair<SubscriptionToken, NotificationDelivery>> store;

        public TestDelivery()
        {
            this.store = new List<KeyValuePair<SubscriptionToken, NotificationDelivery>>();
        }

        public void Send(SubscriptionToken token, NotificationDelivery notification)
        {
            store.Add(new KeyValuePair<SubscriptionToken, NotificationDelivery>(token, notification));
        }

        public void Send(IList<SubscriptionToken> tokens, NotificationDelivery notification)
        {
            foreach (var token in tokens)
            {
                store.Add(new KeyValuePair<SubscriptionToken, NotificationDelivery>(token, notification));
            }
        }

        public List<KeyValuePair<SubscriptionToken, NotificationDelivery>> Store { get { return new List<KeyValuePair<SubscriptionToken, NotificationDelivery>>(store); } }
    }
}
