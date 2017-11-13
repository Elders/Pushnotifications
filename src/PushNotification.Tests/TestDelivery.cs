using System.Collections.Generic;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotification.Tests
{
    public class TestDelivery : IPushNotificationDelivery, IPushNotificationBulkDelivery
    {
        readonly List<KeyValuePair<SubscriptionToken, NotificationDeliveryModel>> store;

        public TestDelivery()
        {
            this.store = new List<KeyValuePair<SubscriptionToken, NotificationDeliveryModel>>();
        }

        public void Send(SubscriptionToken token, NotificationDeliveryModel notification)
        {
            store.Add(new KeyValuePair<SubscriptionToken, NotificationDeliveryModel>(token, notification));
        }

        public void Send(IList<SubscriptionToken> tokens, NotificationDeliveryModel notification)
        {
            foreach (var token in tokens)
            {
                store.Add(new KeyValuePair<SubscriptionToken, NotificationDeliveryModel>(token, notification));
            }
        }

        public List<KeyValuePair<SubscriptionToken, NotificationDeliveryModel>> Store { get { return new List<KeyValuePair<SubscriptionToken, NotificationDeliveryModel>>(store); } }
    }
}
