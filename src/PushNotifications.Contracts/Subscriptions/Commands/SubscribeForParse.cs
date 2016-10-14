using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.Subscriptions.Commands
{
    [DataContract(Name = "e1b6c5b3-8dfe-4bdc-8cdf-7b1d47007197")]
    public class SubscribeForParse : ICommand
    {
        SubscribeForParse() { }

        public SubscribeForParse(ParseSubscriptionId id, string userId, string token)
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

        public bool IsValid()
        {
            return
               Id != null &&
               !string.IsNullOrWhiteSpace(UserId) &&
               !string.IsNullOrWhiteSpace(Token);
        }
    }
}