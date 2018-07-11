using System;
using Elders.Cronus;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class TopicSubscription : AggregateRoot<TopicSubscriptionState>
    {
        TopicSubscription() { }

        public TopicSubscription(TopicSubscriptionId id, SubscriptionType subscryptionType)
        {
            if (id.IsValid() == false) throw new ArgumentException(nameof(id));
            if (ReferenceEquals(null, subscryptionType)) throw new ArgumentNullException(nameof(subscryptionType));

            state = new TopicSubscriptionState();

            IEvent evnt = new SubscribedToTopic(id, subscryptionType);
            Apply(evnt);
        }

        public void SubscribeToTopic(TopicSubscriptionId id, SubscriptionType subscryptionType)
        {
            if (id.IsValid() == false) throw new ArgumentException(nameof(id));
            if (ReferenceEquals(null, subscryptionType)) throw new ArgumentNullException(nameof(subscryptionType));

            if (state.IsSubscriptionActive == false)
            {
                IEvent evnt = new SubscribedToTopic(id, subscryptionType);
                Apply(evnt);
            }
        }

        public void UnsubscribeFromTopic(TopicSubscriptionId id, SubscriptionType subscryptionType)
        {
            if (id.IsValid() == false) throw new ArgumentException(nameof(id));
            if (ReferenceEquals(null, subscryptionType)) throw new ArgumentNullException(nameof(subscryptionType));

            if (state.IsSubscriptionActive)
            {
                IEvent evnt = new UnsubscribedFromTopic(id, subscryptionType);
                Apply(evnt);
            }
        }
    }
}
