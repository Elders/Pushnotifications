using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.FireBaseSubscriptions.Events
{
    [DataContract(Name = "8c3a0c94-9115-43d8-bfa1-ba614bbd07d8")]
    public class SubscriberSubscribedForFireBase : IEvent
    {
        protected SubscriberSubscribedForFireBase() { }

        public SubscriberSubscribedForFireBase(FireBaseSubscriptionId id, SubscriberId userId, SubscriptionToken token)
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
            return $"[FireBase] User '{SubscriberId.Urn.Value}' has subscribed with token '{Token}'. Id: '{Id.Urn.Value}' ";
        }
    }
}
