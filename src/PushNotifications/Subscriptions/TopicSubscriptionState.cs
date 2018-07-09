using Elders.Cronus;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Events;

namespace PushNotifications.Subscriptions
{
    public class TopicSubscriptionState : AggregateRootState<TopicSubscription, TopicSubscriptionId>
    {
        public override TopicSubscriptionId Id { get; set; }

        public SubscriberId SubscriberId { get; set; }

        public Topic Topic { get; set; }

        public bool IsSubscriptionActive { get; set; }

        public void When(SubscribedToTopic e)
        {
            Id = e.Id;
            SubscriberId = e.SubscriberId;
            Topic = e.Topic;
            IsSubscriptionActive = true;
        }

        public void When(UnsubscribedFromTopic e)
        {
            IsSubscriptionActive = false;
        }
    }
}
