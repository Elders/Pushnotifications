using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.Subscriptions;

namespace Multitenancy.Delivery
{
    public interface ITopicSubscriptionProvisioner
    {
        ITopicSubscriptionManager ResolveTopicSubscriptionManager(SubscriptionType subscriptionType, string tenant);
    }
}
