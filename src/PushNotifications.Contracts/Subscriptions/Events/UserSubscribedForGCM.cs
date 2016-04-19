using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "7658948b-01c5-423d-8955-52170787d616")]
    public class UserSubscribedForGCM : IEvent
    {
        UserSubscribedForGCM() { }

        public UserSubscribedForGCM(GCMSubscriptionId id, string userId, string token)
        {
            Id = id;
            UserId = userId;
            Token = token;
        }

        [DataMember(Order = 1)]
        public GCMSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public string UserId { get; private set; }

        [DataMember(Order = 3)]
        public string Token { get; private set; }
    }
}