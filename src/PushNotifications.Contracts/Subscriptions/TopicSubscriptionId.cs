﻿using Elders.Cronus;
using System;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions
{
    [DataContract(Name = "541f8599-28ff-470d-a628-33f68d7178bc")]
    public class TopicSubscriptionId : StringTenantId
    {
        TopicSubscriptionId() { }

        public TopicSubscriptionId(SubscriberId subscriberId, Topic topic, string tenant) : base($"{subscriberId.Id}@@{topic}", "topicSubscription", tenant)
        {
            if (ReferenceEquals(null, subscriberId)) throw new ArgumentNullException(nameof(subscriberId));
            if (ReferenceEquals(null, topic)) throw new ArgumentNullException(nameof(topic));

            Topic = topic;
            SubscriberId = subscriberId;
        }

        [DataMember(Order = 3)]
        public Topic Topic { get; private set; }

        [DataMember(Order = 4)]
        public SubscriberId SubscriberId { get; private set; }

        public bool IsValid()
        {
            if (ReferenceEquals(null, Topic)) return false;
            if (StringTenantId.IsValid(SubscriberId) == false) return false;

            return true;
        }
    }
}
