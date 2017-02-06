using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Commands
{
    [DataContract(Name = "6370f26a-a464-4074-893a-931f6f0a1e9a")]
    public class SubscribeForPushy : ICommand
    {
        SubscribeForPushy() { }

        public SubscribeForPushy(PushySubscriptionId id, string userId, string token)
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

        public bool IsValid()
        {
            return
               Id != null &&
               !string.IsNullOrWhiteSpace(UserId) &&
               !string.IsNullOrWhiteSpace(Token);
        }
    }
}
