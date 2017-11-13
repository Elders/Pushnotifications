using Elders.Cronus.DomainModeling;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public abstract class NotificationDeliveryModel : ValueObject<NotificationDeliveryModel>
    {
        public NotificationDeliveryModel(PushNotificationId pushNotificationId, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
        {
            Id = pushNotificationId;
            NotificationPayload = notificationPayload;
            ExpiresAt = expiresAt;
            ContentAvailable = contentAvailable;
        }

        public PushNotificationId Id { get; private set; }

        public NotificationPayload NotificationPayload { get; private set; }

        public Timestamp ExpiresAt { get; private set; }

        public bool ContentAvailable { get; private set; }
    }
}
