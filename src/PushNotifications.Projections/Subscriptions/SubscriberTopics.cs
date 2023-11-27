using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PushNotifications.Subscriptions;

namespace PushNotifications.Projections.Subscriptions
{
    [DataContract(Name = "c857996a-7baa-4d60-bbce-820df75fb816")]
    public class SubscriberTopics
    {
        public SubscriberTopics()
        {
            Topics = new HashSet<Topic>();
        }

        public SubscriberTopics(DeviceSubscriberId subscriberId) : this()
        {
            if (subscriberId is null == true) throw new ArgumentNullException(nameof(subscriberId));
            SubscriberId = subscriberId;
        }

        public SubscriberTopics(DeviceSubscriberId subscriberId, HashSet<Topic> topics) : this(subscriberId)
        {
            if (topics is null == true) throw new ArgumentNullException(nameof(topics));
            Topics = new HashSet<Topic>();
        }

        [DataMember(Order = 1)]
        public DeviceSubscriberId SubscriberId { get; set; }

        [DataMember(Order = 2)]
        public HashSet<Topic> Topics { get; private set; }
    }
}
