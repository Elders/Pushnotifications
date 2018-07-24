using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "8a3cb4a0-8ec0-48f2-bbc7-5b8c2368fc02")]
    public class SubscribedToTopic : IEvent
    {
        SubscribedToTopic() { }

        public SubscribedToTopic(TopicSubscriptionId topicSubscriptionId, SubscriptionType subscriptionType)
        {
            if (topicSubscriptionId.IsValid() == false) throw new ArgumentNullException(nameof(topicSubscriptionId));
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
            return $"Subscriber '{Id.SubscriberId.Urn.Value}' has subscribed to topic: {Id.Topic}";
        }
    }
}
