using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;
using System;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "9e655fe8-75d2-4481-a358-aec9d24e9e3d")]
    public class UnSubscribed : IEvent
    {
        UnSubscribed() { }

        public UnSubscribed(SubscriptionId id, SubscriberId subscriberId, SubscriptionToken subscriptionToken, SubscriptionType subscriptionType)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (SubscriptionToken.IsValid(subscriptionToken) == false) throw new ArgumentException(nameof(subscriptionToken));
            if (ReferenceEquals(null, subscriptionType) == true) throw new ArgumentNullException(nameof(subscriptionType));

            Id = id;
            SubscriberId = subscriberId;
            SubscriptionToken = subscriptionToken;
            SubscriptionType = subscriptionType;
        }

        [DataMember(Order = 1)]
        public SubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public SubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 3)]
        public SubscriptionToken SubscriptionToken { get; private set; }

        [DataMember(Order = 4)]
        public SubscriptionType SubscriptionType { get; private set; }

        public override string ToString()
        {
            return $"Subscriber '{SubscriberId.Urn.Value}' has unsubscribed with token '{SubscriptionToken}'. Id: '{Id.Urn.Value}' ";
        }
    }
}
