using System.Collections.Generic;
using Elders.Cronus;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Events;

namespace PushNotifications
{
    public class TopicPushNotificationState : AggregateRootState<TopicPushNotification, TopicPushNotificationId>
    {
        public override TopicPushNotificationId Id { get; set; }

        public NotificationPayload NotificationPayload { get; set; }

        public Dictionary<string, object> NotificationData { get; private set; }

        public Timestamp ExpiresAt { get; private set; }

        public bool ContentAvailable { get; private set; }

        public void When(TopicPushNotificationSent e)
        {
            Id = e.Id;
            NotificationPayload = e.NotificationPayload;
            NotificationData = e.NotificationData;
            ExpiresAt = e.ExpiresAt;
            ContentAvailable = e.ContentAvailable;
        }
    }
}
