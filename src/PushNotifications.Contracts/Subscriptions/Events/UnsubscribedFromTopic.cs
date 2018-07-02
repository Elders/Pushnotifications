using Elders.Cronus;
using System.Runtime.Serialization;
using System;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "a67bdb79-4f06-4856-8f04-05f38ce2e6cf")]
    public class UnsubscribedFromTopic : IEvent
    {
        UnsubscribedFromTopic() { }

        public UnsubscribedFromTopic(SubscriptionId id, SubscriberId subscriberId, SubscriptionToken subscriptionToken, string topic)
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
            return $"Subscriber '{SubscriberId.Urn.Value}' has unsubscribed with token '{SubscriptionToken}'. Id: '{Id.Urn.Value}' from Topic: {Topic} ";
        }
    }
}
