using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushySubscriptions.Commands
{
    [DataContract(Name = "ac09557b-2e30-4943-860d-e89d940306f0")]
    public class SubscribeUserForPushy : ICommand
    {
        SubscribeUserForPushy() { }

        public SubscribeUserForPushy(PushySubscriptionId id, SubscriberId userId, SubscriptionToken token)
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
            return $"[Pushy] Subscribe for Pushy - id '{Id.Urn.Value}' userId '{UserId.Urn.Value}' token '{Token}'";
        }
    }
}
