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

        public SendTokensResult Send(SubscriptionToken token, NotificationForDelivery notification)
        {
            return aggregator.Queue(token, notification);
        }

        public SendTokensResult Send(IList<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            foreach (var token in tokens)
            {
                store.Add(new KeyValuePair<SubscriptionToken, NotificationForDelivery>(token, notification));
            }
            return new SendTokensResult(new List<SubscriptionToken>());
        }

        public bool SendToTopic(Topic topic, NotificationForDelivery notification)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<KeyValuePair<SubscriptionToken, NotificationForDelivery>> Store
        {
            get { return store.ToList().AsReadOnly(); }
        }
    }
}
