using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Contracts.Subscriptions
{
    public class FireBaseNotificationDelivery : NotificationDeliveryModel
    {
        public FireBaseNotificationDelivery(PushNotificationId pushNotificationId, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
            : base(pushNotificationId, notificationPayload, expiresAt, contentAvailable)
        {
        }
    }
}
