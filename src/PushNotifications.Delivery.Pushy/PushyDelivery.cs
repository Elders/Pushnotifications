using System.Collections.Generic;
using System.Threading.Tasks;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.PushNotifications;
using PushNotifications.Subscriptions;

namespace PushNotifications.Delivery.Pushy
{
    public class PushyDelivery : IPushNotificationDelivery
    {
        private readonly PushyClient pushy;

        public SubscriptionType Platform => SubscriptionType.Pushy;

        public PushyDelivery(PushyClient pushy)
        {
            this.pushy = pushy;
        }

        public Task<SendTokensResult> SendAsync(IEnumerable<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            return pushy.SendAsync(tokens, notification);
        }

        public Task<bool> SendToTopicAsync(Topic topic, NotificationForDelivery notification)
        {
            return pushy.SendToTopicAsync(topic, notification);
        }
    }
}
