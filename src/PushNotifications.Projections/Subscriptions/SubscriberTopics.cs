using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PushNotifications.Contracts;

namespace PushNotifications.Projections.Subscriptions
{
    [DataContract(Name = "c857996a-7baa-4d60-bbce-820df75fb816")]
    public class SubscriberTopics
    {
        public SubscriberTopics()
        {
            Topics = new HashSet<Topic>();
        }

        public SubscriberTopics(SubscriberId subscriberId) : this()
        {
            if (ReferenceEquals(null, subscriberId) == true) throw new ArgumentNullException(nameof(subscriberId));
            SubscriberId = subscriberId;
        }

        public SubscriberTopics(SubscriberId subscriberId, HashSet<Topic> topics) : this(subscriberId)
        {
            if (ReferenceEquals(null, topics) == true) throw new ArgumentNullException(nameof(topics));
            Topics = new HashSet<Topic>();
        }

        [DataMember(Order = 1)]
        public SubscriberId SubscriberId { get; set; }

        [DataMember(Order = 2)]
        public HashSet<Topic> Topics { get; private set; }
    }
}
