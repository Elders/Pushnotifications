using Elders.Cronus;
using System.Runtime.Serialization;
using System;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "8a3cb4a0-8ec0-48f2-bbc7-5b8c2368fc02")]
    public class SubscribedToTopic : IEvent
    {
        SubscribedToTopic() { }

        public SubscribedToTopic(TopicSubscriptionId id, SubscriberId subscriberId, Topic topic, SubscriptionType subscriptionType)
        {
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (ReferenceEquals(null, subscriptionType)) throw new ArgumentException(nameof(subscriptionType));
            if (ReferenceEquals(null, topic)) throw new ArgumentException(nameof(topic));

            Id = id;
            SubscriberId = subscriberId;
            SubscriptionType = subscriptionType;
            Topic = topic;
        }
        [DataMember(Order = 1)]
        public SubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 2)]
        public Topic Topic { get; private set; }

        [DataMember(Order = 3)]
        public SubscriptionType SubscriptionType { get; private set; }

        [DataMember(Order = 4)]
        public TopicSubscriptionId Id { get; private set; }

        public override string ToString()
        {
            return $"Subscriber '{SubscriberId.Urn.Value}' has subscribed to topic: {Topic}";
        }
    }
}
