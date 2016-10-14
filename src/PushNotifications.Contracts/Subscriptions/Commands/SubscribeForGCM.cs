using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Commands
{
    [DataContract(Name = "8cbdec51-7671-430d-a4a3-437442fefebc")]
    public class SubscribeForGCM : ICommand
    {
        SubscribeForGCM() { }

        public SubscribeForGCM(GCMSubscriptionId id, string userId, string token)
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

        public bool IsValid()
        {
            return
               Id != null &&
               !string.IsNullOrWhiteSpace(UserId) &&
               !string.IsNullOrWhiteSpace(Token);
        }
    }
}