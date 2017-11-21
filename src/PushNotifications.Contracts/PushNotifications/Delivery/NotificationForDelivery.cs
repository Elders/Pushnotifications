using System;
using Elders.Cronus.DomainModeling;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public class NotificationForDelivery : ValueObject<NotificationForDelivery>
    {
        public NotificationForDelivery(PushNotificationId pushNotificationId, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
        {
            if (StringTenantId.IsValid(pushNotificationId) == false) throw new ArgumentException(nameof(pushNotificationId));
            if (ReferenceEquals(null, notificationPayload) == true) throw new ArgumentNullException(nameof(notificationPayload));
            if (ReferenceEquals(null, expiresAt) == true) throw new ArgumentNullException(nameof(expiresAt));

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
