using System;
using Elders.Cronus;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Commands
{
    [DataContract(Name = "96f0a28c-24c9-4ce2-921f-f40d836e4b5a")]
    public class SubscribeToTopic : ICommand
    {
        SubscribeToTopic() { }

        public SubscribeToTopic(SubscriptionId id, SubscriberId subscriberId, SubscriptionToken subscriptionToken, string topic)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (SubscriptionToken.IsValid(subscriptionToken) == false) throw new ArgumentException(nameof(subscriptionToken));
            if (string.IsNullOrEmpty(topic)) throw new ArgumentNullException(nameof(topic));

            Id = id;
            SubscriberId = subscriberId;
            SubscriptionToken = subscriptionToken;
            Topic = topic;
        }

        [DataMember(Order = 1)]
        public SubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public SubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 3)]
        public SubscriptionToken SubscriptionToken { get; private set; }

        [DataMember(Order = 4)]
        public string Topic { get; private set; }

        public override string ToString()
        {
            return $"Subscribe with id '{Id.Urn.Value}' SubscriberId '{SubscriberId.Urn.Value}' token '{SubscriptionToken}' to Topic: {Topic}";
        }
    }
}
