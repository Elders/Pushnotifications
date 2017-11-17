using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotification.Tests
{
    public class AnotherTestNotificationDelivery : NotificationDeliveryModel
    {
        public AnotherTestNotificationDelivery(PushNotificationId pushNotificationId, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
            : base(pushNotificationId, notificationPayload, expiresAt, contentAvailable)
        { }
    }
}