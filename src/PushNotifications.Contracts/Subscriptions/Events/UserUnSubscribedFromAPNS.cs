using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "32ca5e99-5afe-49cb-b097-a701dc08bcae")]
    public class UserUnSubscribedFromAPNS : IEvent
    {
        UserUnSubscribedFromAPNS() { }

        public UserUnSubscribedFromAPNS(APNSSubscriptionId id, string userId, string token)
        {
            Id = id;
            UserId = userId;
            Token = token;
        }

        [DataMember(Order = 1)]
        public APNSSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public string UserId { get; private set; }

        [DataMember(Order = 3)]
        public string Token { get; private set; }
    }
}