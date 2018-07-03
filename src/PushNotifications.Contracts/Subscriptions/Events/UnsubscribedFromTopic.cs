using Elders.Cronus;
using System.Runtime.Serialization;
using System;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "a67bdb79-4f06-4856-8f04-05f38ce2e6cf")]
    public class UnsubscribedFromTopic : IEvent
    {
        UnsubscribedFromTopic() { }

        public UnsubscribedFromTopic(SubscriberId subscriberId, Topic topic)
        {
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (ReferenceEquals(null, topic)) throw new ArgumentException(nameof(topic));

            SubscriberId = subscriberId;
            Topic = topic;
        }

        [DataMember(Order = 1)]
        public SubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 2)]
        public Topic Topic { get; private set; }

        public override string ToString()
        {
            return $"Subscriber '{SubscriberId.Urn.Value}' has unsubscribed from topic: {Topic}";
        }
    }
}
