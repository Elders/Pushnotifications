using Elders.Cronus;
using PushNotifications.Subscriptions;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications
{
    [DataContract(Name = "a798892a-0d80-401c-aa42-d43f7d5634ce")]
    public class TopicPushNotificationId : AggregateRootId
    {
        TopicPushNotificationId() { }

        public TopicPushNotificationId(string tenant, Topic topic, string id) : base(tenant, "topicpushnotification", $"{id}@@{topic}")
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (topic is null) throw new ArgumentNullException(nameof(topic));

            Topic = topic;
        }

        [DataMember(Order = 3)]
        public Topic Topic { get; private set; }

        public bool IsValid()
        {
            if (Topic is null) return false;

            return true;
        }
    }
}
