using System;
using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Subscriptions.Commands
{
    [DataContract(Name = "d8c1dca9-8768-4cb9-b415-b578cfd90ad8")]
    public class UnSubscribe : ICommand
    {
        UnSubscribe()
        {
            Timestamp = DateTimeOffset.UtcNow;
        }

        public UnSubscribe(DeviceSubscriptionId id, DeviceSubscriberId subscriberId, SubscriptionToken subscriptionToken) : this()
        {
            if (id is null) throw new ArgumentException(nameof(id));
            if (subscriberId is null) throw new ArgumentException(nameof(subscriberId));
            if (SubscriptionToken.IsValid(subscriptionToken) == false) throw new ArgumentException(nameof(subscriptionToken)); ;

            Id = id;
            SubscriberId = subscriberId;
            SubscriptionToken = subscriptionToken;
        }

        [DataMember(Order = 1)]
        public DeviceSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public DeviceSubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 3)]
        public SubscriptionToken SubscriptionToken { get; private set; }

        [DataMember(Order = 4)]
        public DateTimeOffset Timestamp { get; private set; }
    }
}
