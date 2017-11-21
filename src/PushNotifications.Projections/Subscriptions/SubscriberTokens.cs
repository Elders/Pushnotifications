using System;
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
            TokenTypePairs = new HashSet<SubscriptionTokenSubscriptionTypePair>();
        }

        public SubscriberTokens(SubscriberId subscriberId) : this()
        {
            if (ReferenceEquals(null, subscriberId) == true) throw new ArgumentNullException(nameof(subscriberId));
            SubscriberId = subscriberId;
        }

        public SubscriberTokens(SubscriberId subscriberId, HashSet<SubscriptionTokenSubscriptionTypePair> tokenTypePairs) : this(subscriberId)
        {
            if (ReferenceEquals(null, tokenTypePairs) == true) throw new ArgumentNullException(nameof(tokenTypePairs));
            TokenTypePairs = new HashSet<SubscriptionTokenSubscriptionTypePair>(tokenTypePairs);
        }

        [DataMember(Order = 1)]
        public SubscriberId SubscriberId { get; set; }

        [DataMember(Order = 2)]
        public HashSet<SubscriptionTokenSubscriptionTypePair> TokenTypePairs { get; private set; }
    }
}
