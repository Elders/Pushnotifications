﻿using Elders.Cronus;
using Elders.Cronus.Projections;
using Microsoft.Extensions.Logging;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Subscriptions;
using PushNotifications.Subscriptions.Commands;
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
        private readonly IPublisher<ICommand> publisher;
        private readonly IProjectionReader projections;
        private readonly MultiPlatformDelivery delivery;
        private readonly ILogger<PushNotificationTrigger> logger;

        public PushNotificationTrigger(IPublisher<ICommand> publisher, IProjectionReader projections, MultiPlatformDelivery delivery, ILogger<PushNotificationTrigger> logger)
        {
            this.publisher = publisher;
            this.projections = projections;
            this.delivery = delivery;
            this.logger = logger;
        }

        public async Task HandleAsync(NotificationMessageSignal signal)
        {
            List<SubscriptionToken> tokens = new List<SubscriptionToken>();
            var tokenToSubscriberList = new List<KeyValuePair<SubscriptionToken, DeviceSubscriberId>>();

            foreach (var recipient in signal.Recipients)
            {
                AggregateRootId urn = AggregateRootId.Parse(recipient.UberDecode());
                DeviceSubscriberId subscriberId = new DeviceSubscriberId(urn.Tenant, urn.Id, signal.Application);

                using (logger.BeginScope(s => s.AddScope("pn_subscriber", subscriberId)))
                {
                    var projectionResult = await projections.GetAsync<SubscriberTokensProjection>(subscriberId);

                    if (projectionResult.IsSuccess)
                    {
                        tokens.AddRange(projectionResult.Data.State.Tokens);

                        foreach (var token in projectionResult.Data.State.Tokens)
                        {
                            tokenToSubscriberList.Add(new KeyValuePair<SubscriptionToken, DeviceSubscriberId>(token, subscriberId));
                        }
                    }
                    else if (projectionResult.HasError)
                    {
                        logger.LogError(projectionResult.Error);
                    }
                }
            }

            if (tokens.Any() == false)
            {
                logger.LogInformation($"No tokens were found for the following recipients:{Environment.NewLine}{string.Join(' ', signal.Recipients)}");
                return;
            }

            NotificationForDelivery notificationForDelivery = signal.ToDelivery();
            PushNotifications.SendTokensResult pushResult = await delivery.SendAsync(tokens, notificationForDelivery);

            if (pushResult.IsSuccessful == false)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (pushResult.IsSuccessful)
                    {
                        break;
                    }
                    await Task.Delay(30000);
                    pushResult = await delivery.SendAsync(tokens, notificationForDelivery);
                    logger.LogWarning($"Retry {i} to sent push notification");
                }
            }

            if (pushResult.IsSuccessful && pushResult.HasFailedTokens)
            {
                logger.LogWarning("Failed to send notification to some of the tokens.");

                foreach (SubscriptionToken failedToken in pushResult.FailedTokens)
                {
                    IEnumerable<KeyValuePair<SubscriptionToken, DeviceSubscriberId>> tokenPair = tokenToSubscriberList.Where(pair => IsSubscriptionTokensEquals(pair.Key, failedToken));
                    foreach (var token in tokenPair)
                    {
                        var subscriberId = token.Value;
                        var deviceSubscriptionId = new DeviceSubscriptionId(signal.Tenant, failedToken.Token);
                        UnSubscribe unSubscribe = new UnSubscribe(deviceSubscriptionId, subscriberId, failedToken);
                        publisher.Publish(unSubscribe);

                        logger.LogDebug($"Unsubscribed the token {failedToken.Token} from the subscriber {subscriberId}");
                    }
                }
            }
        }

        public bool IsSubscriptionTokensEquals(SubscriptionToken token1, SubscriptionToken token2)
        {
            if ((token1.Token.Equals(token2.Token, StringComparison.Ordinal)) && (token1.SubscriptionType == token2.SubscriptionType))
            {
                return true;
            }
            return false;
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
