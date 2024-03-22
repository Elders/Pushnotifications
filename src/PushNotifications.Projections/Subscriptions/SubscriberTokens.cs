using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PushNotifications.Subscriptions;

namespace PushNotifications.Projections.Subscriptions
{
    [DataContract(Name = "4cc819b1-6f29-4cdd-a4d8-0eeb37cb88ec")]
    public class SubscriberTokens
    {
        public SubscriberTokens()
        {
            Tokens = new HashSet<SubscriptionToken>();
        }

        public SubscriberTokens(DeviceSubscriberId subscriberId) : this()
        {
            if (subscriberId is null == true) throw new ArgumentNullException(nameof(subscriberId));
            SubscriberId = subscriberId;
        }

        public SubscriberTokens(DeviceSubscriberId subscriberId, HashSet<SubscriptionToken> tokens) : this(subscriberId)
        {
            if (tokens is null == true) throw new ArgumentNullException(nameof(tokens));
            Tokens = new HashSet<SubscriptionToken>(tokens);
        }

        [DataMember(Order = 1)]
        public DeviceSubscriberId SubscriberId { get; set; }

        [DataMember(Order = 2)]
        public HashSet<SubscriptionToken> Tokens { get; private set; }
    }
}
