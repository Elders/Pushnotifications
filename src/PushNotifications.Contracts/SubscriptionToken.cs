using System.Runtime.Serialization;
using Elders.Cronus.DomainModeling;
using System;

namespace PushNotifications.Contracts
{
    [DataContract(Name = "523c6f6b-afe7-4600-aa14-6866c2627d27")]
    public class SubscriptionToken : ValueObject<SubscriptionToken>
    {
        SubscriptionToken() { }

        public SubscriptionToken(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(SubscriptionToken));
            Token = token;
        }

        [DataMember(Order = 1)]
        string Token { get; set; }

        public override string ToString()
        {
            return Token;
        }

        public static implicit operator string(SubscriptionToken TokenTitle)
        {
            return TokenTitle.Token;
        }

        public static bool IsValid(SubscriptionToken token)
        {
            if (ReferenceEquals(null, token) == true)
                return false;

            return true;
        }
    }
}
