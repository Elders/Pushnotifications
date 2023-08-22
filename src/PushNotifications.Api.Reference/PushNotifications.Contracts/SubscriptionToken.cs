using System.Runtime.Serialization;
using Elders.Cronus;
using System;
using PushNotifications.Contracts.Subscriptions;

namespace PushNotifications.Contracts
{
    [DataContract(Name = "523c6f6b-afe7-4600-aa14-6866c2627d27")]
    public class SubscriptionToken : ValueObject<SubscriptionToken>
    {
        SubscriptionToken() { }

        public SubscriptionToken(string token, SubscriptionType subscriptionType)
        {
            if (string.IsNullOrEmpty(token) == true) throw new ArgumentNullException(nameof(token));
            if (ReferenceEquals(null, subscriptionType) == true) throw new ArgumentNullException(nameof(subscriptionType));

            Token = token;
            SubscriptionType = subscriptionType;
        }

        [DataMember(Order = 1)]
        public string Token { get; private set; }

        [DataMember(Order = 2)]
        public SubscriptionType SubscriptionType { get; private set; }

        public override string ToString()
        {
            return Token;
        }

        public static implicit operator string(SubscriptionToken token)
        {
            return token.ToString();
        }

        public static bool IsValid(SubscriptionToken token)
        {
            if (ReferenceEquals(null, token) == true)
                return false;

            return true;
        }
    }
}
