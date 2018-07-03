using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions
{
    [DataContract(Name = "541f8599-28ff-470d-a628-33f68d7178bc")]
    public class TopicSubscriptionId : StringTenantId
    {
        TopicSubscriptionId() { }

        public TopicSubscriptionId(IUrn urn) : base(urn, "topicSubscription") { }

        public TopicSubscriptionId(string id, string tenant) : base(id, "topicSubscription", tenant) { }
    }
}
