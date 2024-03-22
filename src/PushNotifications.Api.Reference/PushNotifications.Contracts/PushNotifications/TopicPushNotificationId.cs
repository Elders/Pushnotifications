using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications
{
    [DataContract(Name = "a798892a-0d80-401c-aa42-d43f7d5634ce")]
    public class TopicPushNotificationId : StringTenantId
    {
        TopicPushNotificationId() { }

        public TopicPushNotificationId(string id, Topic topic, string tenant) : base($"{id}@@{topic}", "topicpushnotification", tenant)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            Topic = topic;
        }

        [DataMember(Order = 3)]
        public Topic Topic { get; private set; }

        public bool IsValid()
        {
            if (ReferenceEquals(null, Topic)) return false;

            return true;
        }
    }
}
