using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "7c1f107d-2d19-4b9f-a346-c7fdc0abb6a6")]
    public class UserUnSubscribedFromPushy : IEvent
    {
        UserUnSubscribedFromPushy() { }

        public UserUnSubscribedFromPushy(PushySubscriptionId id, string userId, string token)
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
            return "[PUSHY ANDROID] User '" + UserId + "' has unsubscribed from token '" + Token + "'. ";
        }
    }
}
