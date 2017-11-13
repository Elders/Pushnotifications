using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Delivery;

namespace PushNotifications.Contracts.FireBaseSubscriptions
{
    public class FireBaseNotificationDelivery : NotificationDeliveryModel
    {
        public FireBaseNotificationDelivery(PushNotificationId pushNotificationId, NotificationPayload notificationPayload, Timestamp expiresAt, bool contentAvailable)
            : base(pushNotificationId, notificationPayload, expiresAt, contentAvailable)
        {
        }
    }
}
