﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PushNotifications.Contracts;

namespace PushNotifications.Projections.Subscriptions
{
    [DataContract(Name = "4cc819b1-6f29-4cdd-a4d8-0eeb37cb88ec")]
    public class SubscriberTokens
    {
        public SubscriberTokens()
        {
            Tokens = new HashSet<SubscriptionToken>();
        }

        public SubscriberTokens(SubscriberId subscriberId) : this()
        {
            if (ReferenceEquals(null, subscriberId) == true) throw new ArgumentNullException(nameof(subscriberId));
            SubscriberId = subscriberId;
        }

        public SubscriberTokens(SubscriberId subscriberId, HashSet<SubscriptionToken> tokens) : this(subscriberId)
        {
            if (ReferenceEquals(null, tokens) == true) throw new ArgumentNullException(nameof(tokens));
            Tokens = new HashSet<SubscriptionToken>(tokens);
        }

        [DataMember(Order = 1)]
        public SubscriberId SubscriberId { get; set; }

        [DataMember(Order = 2)]
        public HashSet<SubscriptionToken> Tokens { get; private set; }
    }
}
