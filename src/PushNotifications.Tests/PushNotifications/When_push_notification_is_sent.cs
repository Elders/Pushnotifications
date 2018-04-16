using System.Collections.Generic;
using Elders.Cronus;
using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(PushNotification))]
    public class When_push_notification_is_send
    {
        Establish context = () =>
        {
            id = new PushNotificationId("id", "elders");
            subscriberId = new SubscriberId("kv", "elders");
            notificationPayload = new NotificationPayload("the title", "the message body");
            notificationData = new Dictionary<string, object>();
            notificationData.Add("test", "test");
            expiresAt = Timestamp.JudgementDay();
            contentAvailable = true;
        };

        Because of = () => ar = new PushNotification(id, subscriberId, notificationPayload, notificationData, expiresAt, contentAvailable);

        It should_create_push_notification = () => ar.ShouldHaveEvent<PushNotificationSent>(e =>
        {
            e.Id.ShouldEqual(id);
            e.SubscriberId.ShouldEqual(subscriberId);
            e.NotificationPayload.ShouldEqual(notificationPayload);
            e.NotificationData.ShouldEqual(notificationData);
            e.ExpiresAt.ShouldEqual(expiresAt);
            e.ContentAvailable.ShouldEqual(contentAvailable);
        });

        static IAggregateRoot ar;
        static PushNotificationId id;
        static SubscriberId subscriberId;
        static NotificationPayload notificationPayload;
        static Dictionary<string, object> notificationData;
        static Timestamp expiresAt;
        static bool contentAvailable;
    }
}
