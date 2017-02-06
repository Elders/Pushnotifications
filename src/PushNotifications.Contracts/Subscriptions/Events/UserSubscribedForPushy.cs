using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "50ad60d8-0889-4ad8-8185-56f232fb6094")]
    public class UserSubscribedForPushy : IEvent
    {
        UserSubscribedForPushy() { }

        public UserSubscribedForPushy(PushySubscriptionId id, string userId, string token)
        {
            Id = id;
            UserId = userId;
            Token = token;
        }

        [DataMember(Order = 1)]
        public PushySubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public string UserId { get; private set; }

        [DataMember(Order = 3)]
        public string Token { get; private set; }

        public override string ToString()
        {
            return "[PUSHY ANDROID] User '" + UserId + "' has subscribed to token '" + Token + "'. ";
        }
    }
}
