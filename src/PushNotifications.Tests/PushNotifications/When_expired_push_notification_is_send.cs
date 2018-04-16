using System;
using System.Collections.Generic;
using Elders.Cronus;
using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Events;

namespace PushNotifications.Tests.PushNotifications
{
    [Subject(nameof(PushNotification))]
    public class When_expired_push_notification_is_send
    {
        Establish context = () =>
        {
            id = new PushNotificationId("id", "elders");
            subscriberId = new SubscriberId("kv", "elders");
            notificationPayload = new NotificationPayload("the title", "the message body");
            expiresAt = new Timestamp(DateTime.UtcNow.AddDays(-1));
            contentAvailable = true;
            notificationData = new Dictionary<string, object>();
        };

        Because of = () => ar = new PushNotification(id, subscriberId, notificationPayload, notificationData, expiresAt, contentAvailable);

        It should_do_nothing = () => ar.ShouldNotHaveEvent<PushNotificationSent>();

        static IAggregateRoot ar;
        static PushNotificationId id;
        static SubscriberId subscriberId;
        static NotificationPayload notificationPayload;
        static Dictionary<string, object> notificationData;
        static Timestamp expiresAt;
        static bool contentAvailable;
    }
}
