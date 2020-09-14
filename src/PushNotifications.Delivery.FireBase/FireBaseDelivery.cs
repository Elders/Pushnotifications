using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.PushNotifications;
using PushNotifications.Subscriptions;

namespace PushNotifications.Delivery.FireBase
{
    public sealed class FireBaseDelivery : IPushNotificationDelivery
    {
        private readonly FireBaseClient fireBase;
        IPushNotificationAggregator aggregator;

        public FireBaseDelivery(FireBaseClient fireBase)
        {
            this.fireBase = fireBase;
        }

        public SubscriptionType Platform => SubscriptionType.FireBase;

        public Task<SendTokensResult> SendAsync(IEnumerable<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            return fireBase.SendAsync(tokens, notification);
        }

        public bool SendToTopic(Topic topic, NotificationForDelivery notification)
        {
            throw new NotImplementedException();
        }
    }
}
