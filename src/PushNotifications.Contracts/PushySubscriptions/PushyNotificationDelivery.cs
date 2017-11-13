using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Contracts.PushySubscriptions
{
    public class PushyNotificationDelivery : NotificationDeliveryModel
    {
        public PushyNotificationDelivery(PushNotificationId pushNotificationId, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
        : base(pushNotificationId, notificationPayload, expiresAt, contentAvailable)
        { }
    }
}
