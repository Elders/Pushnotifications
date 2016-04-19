using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Events
{
    [DataContract(Name = "3adf1684-b88e-4e54-91c4-c5fc5a88d26c")]
    public class UserSubscribedForParse : IEvent
    {
        UserSubscribedForParse() { }

        public UserSubscribedForParse(ParseSubscriptionId id, string userId, string token)
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