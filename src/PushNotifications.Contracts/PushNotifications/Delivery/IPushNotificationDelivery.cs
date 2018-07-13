namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationDelivery
    {
        SendTokensResult Send(SubscriptionToken token, NotificationForDelivery notification);

        bool SendToTopic(Topic topic, NotificationForDelivery notification);
    }

    public interface ITopicSubscriptionManager
    {
        bool SubscribeToTopic(SubscriptionToken token, Topic topic);

        bool UnsubscribeFromTopic(SubscriptionToken token, Topic topic);
    }
}
