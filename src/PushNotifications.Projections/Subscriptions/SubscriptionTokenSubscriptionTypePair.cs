using System;
using System.Runtime.Serialization;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;

namespace PushNotifications.Projections
{
    [DataContract(Name = "e1f5ddf8-9543-452b-b244-7e3728cacacf")]
    public class SubscriptionTokenSubscriptionTypePair : ValueObject<SubscriptionTokenSubscriptionTypePair>
    {
        public SubscriptionTokenSubscriptionTypePair(SubscriptionToken subscriptionToken, SubscriptionType subscriptionType)
        {
            if (ReferenceEquals(null, subscriptionToken) == true) throw new ArgumentNullException(nameof(subscriptionToken));
            if (ReferenceEquals(null, subscriptionType) == true) throw new ArgumentNullException(nameof(subscriptionType));

            SubscriptionToken = subscriptionToken;
            SubscriptionType = subscriptionType;
        }

        [DataMember(Order = 1)]
        public SubscriptionToken SubscriptionToken { get; private set; }

        [DataMember(Order = 2)]
        public SubscriptionType SubscriptionType { get; private set; }
    }
}
