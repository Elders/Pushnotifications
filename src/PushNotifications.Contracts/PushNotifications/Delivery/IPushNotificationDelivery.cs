namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationDelivery
    {
        bool Send(SubscriptionToken token, NotificationDeliveryModel notification);
    }
}
