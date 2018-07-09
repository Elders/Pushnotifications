using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications
{
    [DataContract(Name = "a798892a-0d80-401c-aa42-d43f7d5634ce")]
    public class TopicPushNotificationId : StringTenantId
    {
        TopicPushNotificationId() { }

        public TopicPushNotificationId(IUrn urn) : base(urn, "topicpushnotification") { }

        public TopicPushNotificationId(string id, string tenant) : base(id, "topicpushnotification", tenant) { }
    }
}
