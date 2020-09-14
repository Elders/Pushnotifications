using PushNotifications.PushNotifications;
using PushNotifications.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public sealed class MultiPlatformDelivery
    {
        Dictionary<string, IPushNotificationDelivery> deliveries;

        public MultiPlatformDelivery(IEnumerable<IPushNotificationDelivery> deliveries)
        {
            this.deliveries = deliveries.ToDictionary(key => key.Platform.ToString());
        }

        public async Task<SendTokensResult> SendAsync(IEnumerable<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            SendTokensResult result = new SendTokensResult(Enumerable.Empty<SubscriptionToken>());

            var tokensGroupedByPlatform = tokens.GroupBy(key => key.SubscriptionType);

            foreach (IGrouping<SubscriptionType, SubscriptionToken> platformTokens in tokensGroupedByPlatform)
            {
                var delivery = deliveries[platformTokens.Key];
                IEnumerable<SubscriptionToken> theTokens = platformTokens.AsEnumerable();
                result = result + await delivery.SendAsync(theTokens, notification); // Do not use += ni**a
            }

            return result;
        }

        public bool SendToTopic(Topic topic, NotificationForDelivery notification)
        {
            throw new NotImplementedException();
        }
    }
}
