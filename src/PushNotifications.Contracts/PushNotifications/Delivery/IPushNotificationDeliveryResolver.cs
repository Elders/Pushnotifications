namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationDeliveryResolver
    {
        IPushNotificationDelivery Resolve(NotificationDeliveryModel notification);
    }
}
