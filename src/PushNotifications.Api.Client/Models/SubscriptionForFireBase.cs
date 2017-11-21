using System;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;

namespace PushNotifications.Api.Client
{
    public class SubscriptionForFireBase
    {
        public SubscriptionForFireBase(StringTenantUrn subscriberUrn, SubscriptionToken token)
        {
            if (ReferenceEquals(subscriberUrn, null) == true) throw new ArgumentNullException(nameof(subscriberUrn));
            if (ReferenceEquals(token, null) == true) throw new ArgumentNullException(nameof(token));

            Tenant = subscriberUrn.Tenant;
            SubscriberUrn = subscriberUrn;
            Token = token;
        }

        public string Tenant { get; private set; }

        public StringTenantUrn SubscriberUrn { get; private set; }

        public SubscriptionToken Token { get; private set; }
    }
}
