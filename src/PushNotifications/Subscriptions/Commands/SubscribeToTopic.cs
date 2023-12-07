using System;
using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions.Commands
{
    [DataContract(Name = "96f0a28c-24c9-4ce2-921f-f40d836e4b5a")]
    public class SubscribeToTopic : ICommand
    {
        SubscribeToTopic()
        {
            Timestamp = DateTimeOffset.UtcNow;
        }

        public SubscribeToTopic(TopicSubscriptionId id, DeviceSubscriberId subscriberId, Topic topic) : this()
        {
            if (id is null) throw new ArgumentException(nameof(id));
            if (id is null) throw new ArgumentException(nameof(subscriberId));
            if (topic is null) throw new ArgumentNullException(nameof(topic));

            Id = id;
            SubscriberId = subscriberId;
            Topic = topic;
        }

        [DataMember(Order = 1)]
        public TopicSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public DeviceSubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 3)]
        public Topic Topic { get; private set; }

        [DataMember(Order = 4)]
        public DateTimeOffset Timestamp { get; private set; }
    }
}
