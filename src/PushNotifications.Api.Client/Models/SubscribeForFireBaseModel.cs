using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;

namespace PushNotifications.Api.Client
{
    public class SubscribeForFireBaseModel
    {
        public SubscribeForFireBaseModel(StringTenantUrn subscriberUrn, SubscriptionToken token)
        {
            Tenant = subscriberUrn.Tenant;
            SubscriberUrn = subscriberUrn;
            Token = token;
        }

        public string Tenant { get; private set; }

        public StringTenantUrn SubscriberUrn { get; private set; }

        public SubscriptionToken Token { get; private set; }
    }
}
