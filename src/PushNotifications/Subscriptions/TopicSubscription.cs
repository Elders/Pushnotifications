using System;
using Elders.Cronus;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class TopicSubscription : AggregateRoot<TopicSubscriptionState>
    {
        TopicSubscription() { }

        public TopicSubscription(TopicSubscriptionId id, SubscriberId subscriberId, Topic topic, SubscriptionType subscryptionType)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (ReferenceEquals(null, subscryptionType)) throw new ArgumentNullException(nameof(subscryptionType));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            state = new TopicSubscriptionState();

            IEvent evnt = new SubscribedToTopic(id, subscriberId, topic, subscryptionType);
            Apply(evnt);
        }

        public void SubscribeToTopic(TopicSubscriptionId id, SubscriberId subscriberId, Topic topic, SubscriptionType subscryptionType)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (ReferenceEquals(null, subscryptionType)) throw new ArgumentNullException(nameof(subscryptionType));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            if (state.IsSubscriptionActive == false || state.SubscriberId != subscriberId)
            {
                IEvent evnt = new SubscribedToTopic(id, subscriberId, topic, subscryptionType);
                Apply(evnt);
            }
        }

        public void UnsubscribeFromTopic(TopicSubscriptionId id, SubscriberId subscriberId, Topic topic, SubscriptionType subscryptionType)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (ReferenceEquals(null, subscryptionType)) throw new ArgumentNullException(nameof(subscryptionType));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            if (state.IsSubscriptionActive)
            {
                IEvent evnt = new UnsubscribedFromTopic(id, subscriberId, topic, subscryptionType);
                Apply(evnt);
            }
        }
    }
}
