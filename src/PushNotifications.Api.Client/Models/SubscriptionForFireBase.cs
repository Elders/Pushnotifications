using System;
using Elders.Cronus.DomainModeling;

namespace PushNotifications.Api.Client
{
    public class SubscriptionForFireBase
    {
        public SubscriptionForFireBase(StringTenantUrn subscriberUrn, string token)
        {
            if (ReferenceEquals(subscriberUrn, null) == true) throw new ArgumentNullException(nameof(subscriberUrn));
            if (string.IsNullOrEmpty(token) == true) throw new ArgumentNullException(nameof(token));

            Tenant = subscriberUrn.Tenant;
            SubscriberUrn = subscriberUrn;
            Token = token;
        }

        public string Tenant { get; private set; }

        public StringTenantUrn SubscriberUrn { get; private set; }

        public string Token { get; private set; }
    }
}
