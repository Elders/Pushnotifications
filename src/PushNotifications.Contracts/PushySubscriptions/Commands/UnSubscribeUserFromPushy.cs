using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushySubscriptions.Commands
{
    [DataContract(Name = "685c1307-f7a3-498f-ad3b-799fa866bf68")]
    public class UnSubscribeUserFromPushy : ICommand
    {
        UnSubscribeUserFromPushy() { }

        public UnSubscribeUserFromPushy(PushySubscriptionId id, SubscriberId userId, SubscriptionToken token)
        {
            Id = id;
            UserId = userId;
            Token = token;
        }

        [DataMember(Order = 1)]
        public PushySubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public SubscriberId UserId { get; private set; }

        [DataMember(Order = 3)]
        public SubscriptionToken Token { get; private set; }

        public bool IsValid()
        {
            return
               StringTenantId.IsValid(Id) && StringTenantId.IsValid(UserId) && SubscriptionToken.IsValid(Token);
        }

        public override string ToString()
        {
            return $"[Pushy] UnSubscribe from Pushy - id '{Id.Urn.Value}' userId '{UserId.Urn.Value}' token '{Token}'";
        }
    }
}
