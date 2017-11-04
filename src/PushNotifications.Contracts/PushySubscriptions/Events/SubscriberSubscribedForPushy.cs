using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushySubscriptions.Events
{
    [DataContract(Name = "893e3e7a-dfe3-4047-8830-d72fe528dfc9")]
    public class SubscriberSubscribedForPushy : IEvent
    {
        protected SubscriberSubscribedForPushy() { }

        public SubscriberSubscribedForPushy(PushySubscriptionId id, SubscriberId userId, SubscriptionToken token)
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
            return $"[Pushy] User '{SubscriberId.Urn.Value}' has subscribed with token '{Token}'. Id: '{Id.Urn.Value}' ";
        }
    }
}
