using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Commands
{
    [DataContract(Name = "548065d0-3b36-4e58-80c2-f4da75357858")]
    public class UnSubscribeFromGCM : ICommand
    {
        UnSubscribeFromGCM() { }

        public UnSubscribeFromGCM(GCMSubscriptionId id, string userId, string token)
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