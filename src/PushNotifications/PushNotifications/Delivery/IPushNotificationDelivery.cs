using PushNotifications.PushNotifications;
using PushNotifications.Subscriptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public interface IPushNotificationDelivery
    {
        public SubscriptionType Platform { get; }

        Task<SendTokensResult> SendAsync(IEnumerable<SubscriptionToken> tokens, NotificationForDelivery notification);

        bool SendToTopic(Topic topic, NotificationForDelivery notification);
    }
}
