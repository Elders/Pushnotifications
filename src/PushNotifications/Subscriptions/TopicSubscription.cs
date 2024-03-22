using System;
using Elders.Cronus;
using PushNotifications.Subscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class TopicSubscription : AggregateRoot<TopicSubscriptionState>
    {
        TopicSubscription() { }

        public TopicSubscription(TopicSubscriptionId id)
        {
            if (id.IsValid() == false) throw new ArgumentException(nameof(id));

            IEvent evnt = new SubscribedToTopic(id, DateTimeOffset.UtcNow);
            Apply(evnt);
        }

        public void SubscribeToTopic(TopicSubscriptionId id, DateTimeOffset timestamp)
        {
            if (id.IsValid() == false) throw new ArgumentException(nameof(id));

            if (state.IsSubscriptionActive == false)
            {
                IEvent evnt = new SubscribedToTopic(id, timestamp);
                Apply(evnt);
            }
        }

        public void UnsubscribeFromTopic(TopicSubscriptionId id, DateTimeOffset timestamp)
        {
            if (id.IsValid() == false) throw new ArgumentException(nameof(id));

            if (state.IsSubscriptionActive)
            {
                IEvent evnt = new UnsubscribedFromTopic(id, timestamp);
                Apply(evnt);
            }
        }
    }
}
