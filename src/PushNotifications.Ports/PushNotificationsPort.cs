using Elders.Cronus;
using Elders.Cronus.Projections;
using Microsoft.Extensions.Logging;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PushNotifications.Ports
{
    public class PushNotificationTrigger : ITrigger,
        ISignalHandle<NotificationMessageSignal>,
        ISignalHandle<TopicNotificationMessageSignal>
    {
        private readonly IProjectionReader projections;
        private readonly MultiPlatformDelivery delivery;
        private readonly ILogger<PushNotificationTrigger> logger;

        public PushNotificationTrigger(IProjectionReader projections, MultiPlatformDelivery delivery, ILogger<PushNotificationTrigger> logger)
        {
            this.projections = projections;
            this.delivery = delivery;
            this.logger = logger;
        }

        public async Task HandleAsync(NotificationMessageSignal signal)
        {
            List<SubscriptionToken> tokens = new List<SubscriptionToken>();

            foreach (var recipient in signal.Recipients)
            {
                AggregateRootId urn = AggregateRootId.Parse(recipient.UberDecode());
                var subscriberId = new DeviceSubscriberId(urn.Tenant, urn.Id, signal.Application);
                using (logger.BeginScope(s => s.AddScope("pn_subscriber", subscriberId)))
                {
                    var projectionResult = await projections.GetAsync<SubscriberTokensProjection>(subscriberId);

                    if (projectionResult.IsSuccess)
                    {
                        tokens.AddRange(projectionResult.Data.State.Tokens);
                    }
                    else if (projectionResult.HasError)
                    {
                        logger.LogError(projectionResult.Error);
                    }
                }
            }

            if (tokens.Any() == false)
            {
                logger.LogInformation($"No tokens were found for the following recipients:{Environment.NewLine}{String.Join(' ', signal.Recipients)}");
                return;
            }

            NotificationForDelivery notificationForDelivery = signal.ToDelivery();
            PushNotifications.SendTokensResult pushResult = await delivery.SendAsync(tokens, notificationForDelivery);
        }

        public async Task HandleAsync(TopicNotificationMessageSignal signal)
        {
            if (signal.Topics.Any() == false)
                logger.LogInformation("No topics were found for the notification");

            NotificationForDelivery notificationForDelivery = signal.ToDelivery();
            foreach (var topica in signal.Topics)
            {
                Topic topic = new Topic(topica);

                bool pushResult = await delivery.SendToTopicAsync(topic, notificationForDelivery);
                LogResult(topic, pushResult);
            }
        }

        private void LogResult(Topic topic, bool pushResult)
        {
            if (pushResult)
            {
                logger.LogInformation($"Notification was sent to topic {topic}");
            }
            else
            {
                logger.LogError($"Failed to send notification to topic {topic}");
            }
        }
    }
}
