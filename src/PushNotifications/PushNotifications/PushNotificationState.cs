using System.Collections.Generic;
using Elders.Cronus;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Events;

namespace PushNotifications
{
    public class PushNotificationState : AggregateRootState<PushNotification, PushNotificationId>
    {
        public override PushNotificationId Id { get; set; }

        public NotificationPayload NotificationPayload { get; set; }

        public Dictionary<string, object> NotificationData { get; private set; }

        public SubscriberId SubscriberId { get; private set; }

        public Timestamp ExpiresAt { get; private set; }

        public bool ContentAvailable { get; private set; }

        public void When(PushNotificationSent e)
        {
            Id = e.Id;
            NotificationPayload = e.NotificationPayload;
            NotificationData = e.NotificationData;
            SubscriberId = e.SubscriberId;
            ExpiresAt = e.ExpiresAt;
            ContentAvailable = e.ContentAvailable;
        }
    }
}
