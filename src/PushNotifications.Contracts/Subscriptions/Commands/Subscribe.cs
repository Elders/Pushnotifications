using System;
using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Commands
{
    [DataContract(Name = "61948e19-1087-4f93-90f3-f39d448cbaba")]
    public class Subscribe : ICommand
    {
        Subscribe()
        { }

        public Subscribe(SubscriptionId id, SubscriberId subscriberId, SubscriptionToken subscriptionToken, SubscriptionType subscriptionType)
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
            return $"Subscribe with id '{Id.Urn.Value}' SubscriberId '{SubscriberId.Urn.Value}' token '{SubscriptionToken}'";
        }
    }
}
