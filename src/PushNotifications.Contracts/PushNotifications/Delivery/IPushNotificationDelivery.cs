namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationDelivery
    {
        void Send(SubscriptionToken token, NotificationDeliveryModel notification);
    }
}
