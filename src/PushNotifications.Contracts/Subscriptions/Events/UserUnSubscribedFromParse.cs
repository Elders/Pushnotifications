using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "1e304121-af0d-41a3-a11f-021aadbb0ff6")]
    public class UserUnSubscribedFromParse : IEvent
    {
        UserUnSubscribedFromParse() { }

        public UserUnSubscribedFromParse(ParseSubscriptionId id, string userId, string token)
        {
            Id = id;
            UserId = userId;
            Token = token;
        }

        [DataMember(Order = 1)]
        public ParseSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public string UserId { get; private set; }

        [DataMember(Order = 3)]
        public string Token { get; private set; }
    }
}