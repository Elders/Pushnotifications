using Elders.Cronus.DomainModeling;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public class NotificationDelivery : ValueObject<NotificationDelivery>
    {
        public NotificationDelivery(NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
        {
            NotificationPayload = notificationPayload;
            ExpiresAt = expiresAt;
            ContentAvailable = contentAvailable;
        }

        public NotificationPayload NotificationPayload { get; private set; }

        public Timestamp ExpiresAt { get; private set; }

        public bool ContentAvailable { get; private set; }
    }
}
