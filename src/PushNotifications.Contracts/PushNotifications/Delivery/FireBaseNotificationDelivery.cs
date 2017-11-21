namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public class FireBaseNotificationDelivery : NotificationDeliveryModel
    {
        public FireBaseNotificationDelivery(PushNotificationId pushNotificationId, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
            : base(pushNotificationId, notificationPayload, expiresAt, contentAvailable)
        {
        }
    }
}
