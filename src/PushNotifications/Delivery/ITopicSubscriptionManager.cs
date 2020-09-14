using PushNotifications.Subscriptions;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface ITopicSubscriptionManager
    {
        bool SubscribeToTopic(SubscriptionToken token, Topic topic);

        bool UnsubscribeFromTopic(SubscriptionToken token, Topic topic);
    }
}
