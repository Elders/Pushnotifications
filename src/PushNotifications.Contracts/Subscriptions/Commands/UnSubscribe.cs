using System;
using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Commands
{
    [DataContract(Name = "d8c1dca9-8768-4cb9-b415-b578cfd90ad8")]
    public class UnSubscribe : ICommand
    {
        UnSubscribe() { }

        public UnSubscribe(SubscriptionId id, SubscriberId subscriberId, SubscriptionToken subscriptionToken)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (SubscriptionToken.IsValid(subscriptionToken) == false) throw new ArgumentException(nameof(subscriptionToken)); ;

            Id = id;
            SubscriberId = subscriberId;
            SubscriptionToken = subscriptionToken;
        }

        [DataMember(Order = 1)]
        public SubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public SubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 3)]
        public SubscriptionToken SubscriptionToken { get; private set; }

        public override string ToString()
        {
            return $"UnSubscribe with id '{Id.Urn.Value}' SubscriberId '{SubscriberId.Urn.Value}' token '{SubscriptionToken}'";
        }
    }
}
