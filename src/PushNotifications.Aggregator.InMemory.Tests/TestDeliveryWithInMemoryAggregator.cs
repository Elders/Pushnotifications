using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.PushNotifications;
using PushNotifications.Subscriptions;

namespace PushNotifications.Aggregator.InMemory.Tests
{
    public class TestDeliveryWithInMemoryAggregator : IPushNotificationDelivery
    {
        InMemoryPushNotificationAggregator aggregator;

        readonly ConcurrentBag<KeyValuePair<IEnumerable<SubscriptionToken>, NotificationForDelivery>> store;

        public TestDeliveryWithInMemoryAggregator(TimeSpan timeSpan, int recipientsCountBeforeFlush)
        {
            aggregator = new InMemoryPushNotificationAggregator(SendAsync, timeSpan, recipientsCountBeforeFlush);
            this.store = new ConcurrentBag<KeyValuePair<IEnumerable<SubscriptionToken>, NotificationForDelivery>>();
        }

        public Task<SendTokensResult> SendAsync(IEnumerable<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            store.Add(new KeyValuePair<IEnumerable<SubscriptionToken>, NotificationForDelivery>(tokens, notification));

            return Task.FromResult(new SendTokensResult(new List<SubscriptionToken>()));
        }

        public bool SendToTopic(Topic topic, NotificationForDelivery notification)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<KeyValuePair<IEnumerable<SubscriptionToken>, NotificationForDelivery>> Store
        {
            get { return store.ToList().AsReadOnly(); }
        }

        public SubscriptionType Platform => SubscriptionType.Create("test");
    }
}
