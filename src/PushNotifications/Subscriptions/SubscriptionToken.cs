using System.Runtime.Serialization;
using System;

namespace PushNotifications.Subscriptions
{
    [DataContract(Name = "523c6f6b-afe7-4600-aa14-6866c2627d27")]
    public sealed record class SubscriptionToken
    {
        SubscriptionToken() { }

        public SubscriptionToken(string token, SubscriptionType subscriptionType)
        {
            if (string.IsNullOrEmpty(token) == true) throw new ArgumentNullException(nameof(token));
            if (subscriptionType is null == true) throw new ArgumentNullException(nameof(subscriptionType));

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
            if (token is null == true)
                return false;

            return true;
        }
    }
}
