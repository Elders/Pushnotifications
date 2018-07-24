using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "a67bdb79-4f06-4856-8f04-05f38ce2e6cf")]
    public class UnsubscribedFromTopic : IEvent
    {
        UnsubscribedFromTopic() { }

        public UnsubscribedFromTopic(TopicSubscriptionId topicSubscriptionId, SubscriptionType subscriptionType)
        {
            if (topicSubscriptionId.IsValid() == false) throw new ArgumentException(nameof(topicSubscriptionId));
            if (ReferenceEquals(null, subscriptionType)) throw new ArgumentException(nameof(subscriptionType));

            Id = topicSubscriptionId;
            SubscriptionType = subscriptionType;
        }

        [DataMember(Order = 1)]
        public TopicSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public SubscriptionType SubscriptionType { get; private set; }

        public override string ToString()
        {
            return $"Subscriber '{Id.SubscriberId.Urn.Value}' has unsubscribed from topic: {Id.Topic}";
        }
    }
}
