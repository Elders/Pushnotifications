namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public class NotificationDelivery
    {
        public NotificationDelivery(SubscriptionToken token, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
        {
            Token = token;
            NotificationPayload = notificationPayload;
            ExpiresAt = expiresAt;
            ContentAvailable = contentAvailable;
        }

        public SubscriptionToken Token { get; private set; }

        public NotificationPayload NotificationPayload { get; private set; }

        public Timestamp ExpiresAt { get; private set; }

        public bool ContentAvailable { get; private set; }
    }
}
