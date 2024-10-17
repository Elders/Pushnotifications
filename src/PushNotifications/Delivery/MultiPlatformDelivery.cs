using Elders.Cronus;
using Microsoft.Extensions.Logging;
using PushNotifications.PushNotifications;
using PushNotifications.Subscriptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public sealed class MultiPlatformDelivery
    {
        private readonly ILogger<MultiPlatformDelivery> logger;
        Dictionary<string, IPushNotificationDelivery> deliveries;

        public MultiPlatformDelivery(IEnumerable<IPushNotificationDelivery> deliveries, ILogger<MultiPlatformDelivery> logger)
        {
            this.deliveries = deliveries.ToDictionary(key => key.Platform.ToString());
            this.logger = logger;
        }

        public async Task<SendTokensResult> SendAsync(IEnumerable<SubscriptionToken> tokens, NotificationForDelivery notification)
        {
            logger.LogInformation("Start sending push notifications...");

            SendTokensResult result = SendTokensResult.Success;

            var tokensGroupedByPlatform = tokens.GroupBy(key => key.SubscriptionType);

            foreach (IGrouping<SubscriptionType, SubscriptionToken> platformTokens in tokensGroupedByPlatform)
            {
                var delivery = deliveries[platformTokens.Key];
                IEnumerable<SubscriptionToken> theTokens = platformTokens.AsEnumerable();
                var localResult = await delivery.SendAsync(theTokens, notification).ConfigureAwait(false);
                logger.Info(() => $"PN send has failed? {localResult.HasFailedTokens}");
                result = result + localResult; // Do not use += ni**a
            }

            return result;
        }

        public async Task<bool> SendToTopicAsync(Topic topic, NotificationForDelivery notification)
        {
            bool result = true;
            foreach (IPushNotificationDelivery delivery in deliveries.Values)
            {
                result &= await delivery.SendToTopicAsync(topic, notification).ConfigureAwait(false);
            }

            return result;
        }
    }
}
