using System;
using Elders.Cronus;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class TopicSubscription : AggregateRoot<TopicSubscriptionState>
    {
        public TopicSubscription(SubscriberId subscriberId, Topic topic)
        {
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            state = new TopicSubscriptionState();

            IEvent evnt = new SubscribedToTopic(subscriberId, topic);
            Apply(evnt);
        }

        public void SubscribeToTopic(SubscriberId subscriberId, Topic topic)
        {
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            if (state.IsSubscriptionActive == false || state.SubscriberId != subscriberId)
            {
                IEvent evnt = new SubscribedToTopic(subscriberId, topic);
                Apply(evnt);
            }
        }

        public void UnsubscribeFromTopic(SubscriberId subscriberId, Topic topic)
        {
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            if (state.IsSubscriptionActive)
            {
                IEvent evnt = new UnsubscribedFromTopic(subscriberId, topic);
                Apply(evnt);
            }
        }
    }
}
