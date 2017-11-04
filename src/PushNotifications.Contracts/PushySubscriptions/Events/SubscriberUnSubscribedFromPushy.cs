using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushySubscriptions.Events
{
    [DataContract(Name = "5fa85387-9591-48a6-bb79-b63919ee75f5")]
    public class SubscriberUnSubscribedFromPushy : IEvent
    {
        protected SubscriberUnSubscribedFromPushy() { }

        public SubscriberUnSubscribedFromPushy(PushySubscriptionId id, SubscriberId userId, SubscriptionToken token)
        {
            Id = id;
            SubscriberId = userId;
            Token = token;
        }

        [DataMember(Order = 1)]
        public PushySubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public SubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 3)]
        public SubscriptionToken Token { get; private set; }

        public override string ToString()
        {
            return $"[Pushy] User '{SubscriberId.Urn.Value}' has unsubscribed with token '{Token}'. Id: '{Id.Urn.Value}' ";
        }
    }
}
