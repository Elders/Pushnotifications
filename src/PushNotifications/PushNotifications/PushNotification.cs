using System;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Events;

namespace PushNotifications
{
    public class PushNotification : AggregateRoot<PushNotificationState>
    {
        PushNotification() { }

        public PushNotification(PushNotificationId id, SubscriberId subscriberId, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
        {
            if (StringTenantId.IsValid(id) == false) throw new ArgumentException(nameof(id));
            if (StringTenantId.IsValid(subscriberId) == false) throw new ArgumentException(nameof(subscriberId));
            if (ReferenceEquals(null, notificationPayload) == true) throw new ArgumentException(nameof(notificationPayload));
            if (ReferenceEquals(null, expiresAt) == true) throw new ArgumentException(nameof(expiresAt));

            state = new PushNotificationState();

            // Ignore pushnotifications that expired
            if (Timestamp.UtcNow().DateTime > expiresAt.DateTime)
                return;

            IEvent @event = new PushNotificationSent(id, subscriberId, notificationPayload, expiresAt, contentAvailable);
            Apply(@event);
        }
    }
}
