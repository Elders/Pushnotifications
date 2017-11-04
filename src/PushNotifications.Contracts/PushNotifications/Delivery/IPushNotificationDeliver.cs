namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationDeliver
    {
        void Send(NotificationDelivery notification);
    }
}