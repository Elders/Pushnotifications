using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.FireBaseSubscriptions.Events
{
    [DataContract(Name = "336875b3-1180-4e61-a144-9f62294c07de")]
    public class UserUnSubscribedFromFireBase : IEvent
    {
        protected UserUnSubscribedFromFireBase() { }

        public UserUnSubscribedFromFireBase(FireBaseSubscriptionId id, SubscriberId userId, SubscriptionToken token)
        {
            Id = id;
            UserId = userId;
            Token = token;
        }

        [DataMember(Order = 1)]
        public FireBaseSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public SubscriberId UserId { get; private set; }

        [DataMember(Order = 3)]
        public SubscriptionToken Token { get; private set; }

        public override string ToString()
        {
            return $"[FireBase] User '{UserId.Urn.Value}' has unsubscribed with token '{Token}'. Id: '{Id.Urn.Value}' ";
        }
    }
}
