namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public class PushyNotificationDelivery : NotificationDeliveryModel
    {
        public PushyNotificationDelivery(PushNotificationId pushNotificationId, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
            : base(pushNotificationId, notificationPayload, expiresAt, contentAvailable)
        {
        }
    }
}
