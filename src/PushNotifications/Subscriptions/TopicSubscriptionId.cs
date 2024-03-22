using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions
{
    [DataContract(Name = "541f8599-28ff-470d-a628-33f68d7178bc")]
    public class TopicSubscriptionId : AggregateRootId
    {
        TopicSubscriptionId() { }

        public TopicSubscriptionId(string tenant, Topic topic, DeviceSubscriberId subscriberId) : base(tenant, "topicSubscription", $"{subscriberId.Id}@@{topic}")
        {
            if (subscriberId is null) throw new ArgumentNullException(nameof(subscriberId));
            if (topic is null) throw new ArgumentNullException(nameof(topic));

            Topic = topic;
            SubscriberId = subscriberId;
        }

        [DataMember(Order = 3)]
        public Topic Topic { get; private set; }

        [DataMember(Order = 4)]
        public DeviceSubscriberId SubscriberId { get; private set; }

        public bool IsValid()
        {
            if (Topic is null || SubscriberId is null) return false;

            return true;
        }
    }
}
