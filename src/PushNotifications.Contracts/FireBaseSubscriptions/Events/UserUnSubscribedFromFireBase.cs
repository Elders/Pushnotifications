using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.FireBaseSubscriptions.Events
{
    [DataContract(Name = "336875b3-1180-4e61-a144-9f62294c07de")]
    public class SubscriberUnSubscribedFromFireBase : IEvent
    {
        protected SubscriberUnSubscribedFromFireBase() { }

        public SubscriberUnSubscribedFromFireBase(FireBaseSubscriptionId id, SubscriberId userId, SubscriptionToken token)
        {
            Id = id;
            SubscriberId = userId;
            Token = token;
        }

        [DataMember(Order = 1)]
        public FireBaseSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public SubscriberId SubscriberId { get; private set; }

        [DataMember(Order = 3)]
        public SubscriptionToken Token { get; private set; }

        public override string ToString()
        {
            return $"[FireBase] User '{SubscriberId.Urn.Value}' has unsubscribed with token '{Token}'. Id: '{Id.Urn.Value}' ";
        }
    }
}
