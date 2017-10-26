using System;
using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.FireBaseSubscriptions.Commands
{
    [DataContract(Name = "a22216c5-a75e-4fde-ac68-87baf8ecc2ab")]
    public class UnSubscribeUserFromFireBase : ICommand
    {
        UnSubscribeUserFromFireBase() { }

        public UnSubscribeUserFromFireBase(FireBaseSubscriptionId id, UserId userId, SubscriptionToken token)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(userId) == false) throw new ArgumentException(nameof(userId));
            if (SubscriptionToken.IsValid(token) == false) throw new ArgumentException(nameof(token));

            Id = id;
            UserId = userId;
            Token = token;
        }

        [DataMember(Order = 1)]
        public FireBaseSubscriptionId Id { get; private set; }

        [DataMember(Order = 2)]
        public UserId UserId { get; private set; }

        [DataMember(Order = 3)]
        public SubscriptionToken Token { get; private set; }

        public bool IsValid()
        {
            return
               StringTenantId.IsValid(Id) && StringTenantId.IsValid(UserId) && SubscriptionToken.IsValid(Token);
        }

        public override string ToString()
        {
            return $"[FireBase] UnSubscribe from Fire Base - id '{Id.Urn.Value}' userId '{UserId.Urn.Value}' token '{Token}'";
        }
    }
}
