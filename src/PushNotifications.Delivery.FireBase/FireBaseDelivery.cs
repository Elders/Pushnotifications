using System.Collections.Generic;
using System.Threading.Tasks;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.PushNotifications;
using PushNotifications.Subscriptions;

namespace PushNotifications.Delivery.FireBase
{
    public sealed class FireBaseDelivery : IPushNotificationDelivery
    {
        private readonly FirebaseNotificationService firebaseNotificationService;

        public FireBaseDelivery(FirebaseNotificationService firebaseNotificationService)
        {
            this.firebaseNotificationService = firebaseNotificationService;
        }

        public SubscriptionType Platform => SubscriptionType.FireBase;

        public async Task<SendTokensResult> SendAsync(IEnumerable<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            return await firebaseNotificationService.SendNotificationsAsync(tokens, notification);
        }

        public Task<bool> SendToTopicAsync(Topic topic, NotificationForDelivery notification)
        {
            return firebaseNotificationService.SendToTopicAsync(topic, notification);
        }
    }
}
