using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "a67bdb79-4f06-4856-8f04-05f38ce2e6cf")]
    public class UnsubscribedFromTopic : IEvent
    {
        UnsubscribedFromTopic() { }

        public UnsubscribedFromTopic(TopicSubscriptionId topicSubscriptionId)
        {
            if (topicSubscriptionId.IsValid() == false) throw new ArgumentException(nameof(topicSubscriptionId));

            Id = topicSubscriptionId;
        }

        [DataMember(Order = 1)]
        public TopicSubscriptionId Id { get; private set; }

        public override string ToString()
        {
            return $"Subscriber '{Id.SubscriberId.Urn.Value}' has unsubscribed from topic: {Id.Topic}";
        }
    }
}
