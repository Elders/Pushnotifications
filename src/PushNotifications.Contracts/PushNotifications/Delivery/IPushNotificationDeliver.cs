namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationDeliver
    {
        void Send(SubscriptionToken token, NotificationDelivery notification);
    }
}
