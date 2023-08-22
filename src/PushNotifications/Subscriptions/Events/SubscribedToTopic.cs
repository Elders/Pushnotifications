using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions.Events
{
    [DataContract(Name = "8a3cb4a0-8ec0-48f2-bbc7-5b8c2368fc02")]
    public class SubscribedToTopic : IEvent
    {
        SubscribedToTopic() { }

        public SubscribedToTopic(TopicSubscriptionId topicSubscriptionId)
        {
            if (topicSubscriptionId.IsValid() == false) throw new ArgumentNullException(nameof(topicSubscriptionId));

            Id = topicSubscriptionId;
        }

        [DataMember(Order = 1)]
        public TopicSubscriptionId Id { get; private set; }
    }
}
