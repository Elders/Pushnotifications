using Elders.Cronus.DomainModeling;
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
            expiresAt = Timestamp.JudgementDay();
            contentAvailable = true;
        };

        Because of = () => ar = new PushNotification(id, subscriberId, notificationPayload, expiresAt, contentAvailable);

        It should_create_push_notification = () => ar.ShouldHaveEvent<PushNotificationSent>(e =>
        {
            e.Id.ShouldEqual(id);
            e.SubscriberId.ShouldEqual(subscriberId);
            e.NotificationPayload.ShouldEqual(notificationPayload);
            e.ExpiresAt.ShouldEqual(expiresAt);
            e.ContentAvailable.ShouldEqual(contentAvailable);
        });

        static IAggregateRoot ar;
        static PushNotificationId id;
        static SubscriberId subscriberId;
        static NotificationPayload notificationPayload;
        static Timestamp expiresAt;
        static bool contentAvailable;
    }
}
