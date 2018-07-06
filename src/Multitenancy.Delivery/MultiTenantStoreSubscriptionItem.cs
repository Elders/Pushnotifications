using System;
using Elders.Cronus;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.Subscriptions;

namespace Multitenancy.Delivery
{
    public class MultiTenantStoreSubscriptionItem : ValueObject<MultiTenantStoreItem>
    {
        public MultiTenantStoreSubscriptionItem(string tenant, SubscriptionType subscriptionType, ITopicSubscriptionManager topicSubscriptionManager)
        {
            if (string.IsNullOrEmpty(tenant) == true) throw new ArgumentNullException(nameof(tenant));
            if (ReferenceEquals(null, subscriptionType) == true) throw new ArgumentNullException(nameof(subscriptionType));
            if (ReferenceEquals(null, topicSubscriptionManager) == true) throw new ArgumentNullException(nameof(topicSubscriptionManager));

            Tenant = tenant;
            SubscriptionType = subscriptionType;
            TopicSubscriptionManager = topicSubscriptionManager;
        }

        public string Tenant { get; private set; }

        public SubscriptionType SubscriptionType { get; private set; }

        public ITopicSubscriptionManager TopicSubscriptionManager { get; private set; }
    }
}
