using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions
{
    [DataContract(Name = "541f8599-28ff-470d-a628-33f68d7178bc")]
    public class TopicSubscriptionId : StringTenantId
    {
        TopicSubscriptionId() { }

        public TopicSubscriptionId(IUrn urn) : base(urn, "topicSubscription") { }

        public TopicSubscriptionId(SubscriberId subscriberId, Topic topic, string tenant) : base($"{subscriberId.Id}@@{topic}", "topicSubscription", tenant)
        {
            Topic = topic;
        }

        [DataMember(Order = 3)]
        public Topic Topic { get; private set; }
    }
}
