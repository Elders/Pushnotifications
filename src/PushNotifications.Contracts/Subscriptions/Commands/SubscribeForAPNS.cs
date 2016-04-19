using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Commands
{
    [DataContract(Name = "3f2ee7d9-7b12-458f-af51-a5a7a937949a")]
    public class SubscribeForAPNS : ICommand
    {
        SubscribeForAPNS() { }

        public SubscribeForAPNS(APNSSubscriptionId id, string userId, string token)
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

        public bool IsValid()
        {
            return
               Id != null &&
               !string.IsNullOrWhiteSpace(UserId) &&
               !string.IsNullOrWhiteSpace(Token);
        }
    }
}