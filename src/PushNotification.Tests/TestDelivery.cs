using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotification.Tests
{
    public class TestDelivery : IPushNotificationDelivery, IPushNotificationBufferedDelivery
    {
        readonly ConcurrentBag<KeyValuePair<SubscriptionToken, NotificationForDelivery>> store;

        public TestDelivery()
        {
            this.store = new ConcurrentBag<KeyValuePair<SubscriptionToken, NotificationForDelivery>>();
        }

        public bool Send(SubscriptionToken token, NotificationForDelivery notification)
        {
            store.Add(new KeyValuePair<SubscriptionToken, NotificationForDelivery>(token, notification));
            return true;
        }

        public bool Send(IList<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            foreach (var token in tokens)
            {
                store.Add(new KeyValuePair<SubscriptionToken, NotificationForDelivery>(token, notification));
            }
            return true;
        }

        public ReadOnlyCollection<KeyValuePair<SubscriptionToken, NotificationForDelivery>> Store
        {
            get { return store.ToList().AsReadOnly(); }
        }
    }
}
