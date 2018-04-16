using System;
using System.Collections.Generic;
using Elders.Cronus;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public class NotificationForDelivery : ValueObject<NotificationForDelivery>
    {
        public NotificationForDelivery(PushNotificationId pushNotificationId, NotificationPayload notificationPayload, Dictionary<string, object> notificationData, Timestamp expiresAt, bool contentAvailable)
        {
            if (StringTenantId.IsValid(pushNotificationId) == false) throw new ArgumentException(nameof(pushNotificationId));
            if (ReferenceEquals(null, notificationPayload) == true) throw new ArgumentNullException(nameof(notificationPayload));
            if (ReferenceEquals(null, notificationData) == true) throw new ArgumentNullException(nameof(notificationData));
            if (ReferenceEquals(null, expiresAt) == true) throw new ArgumentNullException(nameof(expiresAt));

            Id = pushNotificationId;
            NotificationPayload = notificationPayload;
            NotificationData = notificationData;
            ExpiresAt = expiresAt;
            ContentAvailable = contentAvailable;
        }

        public PushNotificationId Id { get; private set; }

        public NotificationPayload NotificationPayload { get; private set; }

        public Dictionary<string, object> NotificationData { get; private set; }

        public Timestamp ExpiresAt { get; private set; }

        public bool ContentAvailable { get; private set; }
    }
}
