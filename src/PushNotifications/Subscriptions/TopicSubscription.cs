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

            state = new TopicSubscriptionState();

            IEvent evnt = new SubscribedToTopic(id);
            Apply(evnt);
        }

        public void SubscribeToTopic(TopicSubscriptionId id)
        {
            if (id.IsValid() == false) throw new ArgumentException(nameof(id));

            if (state.IsSubscriptionActive == false)
            {
                IEvent evnt = new SubscribedToTopic(id);
                Apply(evnt);
            }
        }

        public void UnsubscribeFromTopic(TopicSubscriptionId id)
        {
            if (id.IsValid() == false) throw new ArgumentException(nameof(id));

            if (state.IsSubscriptionActive)
            {
                IEvent evnt = new UnsubscribedFromTopic(id);
                Apply(evnt);
            }
        }
    }
}
