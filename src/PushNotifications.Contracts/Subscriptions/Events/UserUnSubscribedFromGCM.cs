using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "b4d7d41a-03ce-4b65-8cab-8489fd956e27")]
    public class UserUnSubscribedFromGCM : IEvent
    {
        UserUnSubscribedFromGCM() { }

        public UserUnSubscribedFromGCM(GCMSubscriptionId id, string userId, string token)
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

        public override string ToString()
        {
            return "[GCM] User '" + UserId + "' has unsubscribed from token '" + Token + "'. ";
        }
    }
}