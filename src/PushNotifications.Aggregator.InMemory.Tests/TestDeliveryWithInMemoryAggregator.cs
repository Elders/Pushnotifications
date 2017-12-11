using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Aggregator.InMemory.Tests
{
    public class TestDeliveryWithInMemoryAggregator : IPushNotificationDelivery
    {
        InMemoryPushNotificationAggregator aggregator;

        readonly ConcurrentBag<KeyValuePair<SubscriptionToken, NotificationForDelivery>> store;

        public TestDeliveryWithInMemoryAggregator(TimeSpan timeSpan, int recipientsCountBeforeFlush)
        {
            aggregator = new InMemoryPushNotificationAggregator(Send, timeSpan, recipientsCountBeforeFlush);
            this.store = new ConcurrentBag<KeyValuePair<SubscriptionToken, NotificationForDelivery>>();
        }

        public bool Send(SubscriptionToken token, NotificationForDelivery notification)
        {
            return aggregator.Queue(token, notification);
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
