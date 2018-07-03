using System;
using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Commands
{
    [DataContract(Name = "96f0a28c-24c9-4ce2-921f-f40d836e4b5a")]
    public class SubscribeToTopic : ICommand
    {
        SubscribeToTopic() { }

        public SubscribeToTopic(TopicSubscriptionId id, SubscriberId subscriberId, Topic topic)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            Id = id;
            SubscriberId = subscriberId;
            Topic = topic;
        }

        [DataMember(Order = 1)]
        public TopicSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public SubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 3)]
        public Topic Topic { get; private set; }

        public override string ToString()
        {
            return $"Subscribe SubscriberId '{SubscriberId.Urn.Value}' to Topic: {Topic}";
        }
    }
}
