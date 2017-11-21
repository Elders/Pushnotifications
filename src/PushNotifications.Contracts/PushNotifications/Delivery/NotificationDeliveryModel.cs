using System;
using System.Collections.Generic;
using Elders.Cronus.DomainModeling;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public abstract class NotificationDeliveryModel : IEqualityComparer<NotificationDeliveryModel>, IEquatable<NotificationDeliveryModel>
    {
        public NotificationDeliveryModel(PushNotificationId pushNotificationId, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
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

        public bool Equals(NotificationDeliveryModel x, NotificationDeliveryModel y)
        {
            if (ReferenceEquals(null, y)) return false;
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(null, y.Id)) return false;
            if (ReferenceEquals(null, Id)) return false;
            if (x.GetType() != y.GetType()) return false;

            return x.Id.Equals(y.Id);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 103 ^ Id.GetHashCode();
                return result;
            }
        }

        public int GetHashCode(NotificationDeliveryModel obj)
        {
            return obj.GetHashCode();
        }

        public bool Equals(NotificationDeliveryModel other)
        {
            return Equals(this, other);
        }
    }
}
