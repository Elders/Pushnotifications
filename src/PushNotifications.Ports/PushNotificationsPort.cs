using System;
using Elders.Cronus;
using Elders.Cronus.Projections;
using Multitenancy.Delivery;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.PushNotifications.Events;
using PushNotifications.Ports.Logging;
using PushNotifications.Projections.Subscriptions;
using PushNotifications.Contracts.Subscriptions.Commands;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications;

namespace PushNotifications.Ports
{
    public class PushNotificationsPort : IPort,
        IEventHandler<PushNotificationSent>,
        IEventHandler<TopicPushNotificationSent>
    {
        static ILog log = LogProvider.GetLogger(typeof(PushNotificationsPort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IProjectionLoader Projections { get; set; }

        public IDeliveryProvisioner DeliveryProvisioner { get; set; }

        public IBadgeCountTrackerFactory BadgeCountTrackerFactory { get; set; }

        public void Handle(PushNotificationSent @event)
        {
            if (ReferenceEquals(null, Projections)) throw new ArgumentNullException(nameof(Projections));
            if (ReferenceEquals(null, DeliveryProvisioner)) throw new ArgumentNullException(nameof(DeliveryProvisioner));
            if (ReferenceEquals(null, CommandPublisher)) throw new ArgumentNullException(nameof(CommandPublisher));

            string tenant = @event.Id.Tenant;
            string subscriberId = @event.SubscriberId.Urn.Value;

            var projectionReponse = Projections.Get<SubscriberTokensProjection>(@event.SubscriberId);
            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No tokens were found for subscriber {@event.SubscriberId}");
                return;
            }

            NotificationPayload notificationPayload = SetNotificationPayloadBadgeCount(@event.NotificationPayload, tenant, subscriberId);

            foreach (var token in projectionReponse.Projection.State.Tokens)
            {
                var notification = new NotificationForDelivery(@event.Id, notificationPayload, @event.NotificationData, @event.ExpiresAt, @event.ContentAvailable);
                var delivery = DeliveryProvisioner.ResolveDelivery(token.SubscriptionType, notification);
                SendTokensResult sendResult = delivery.Send(token, notification);

                if (sendResult.HasFailedTokens)
                {
                    foreach (var failedToken in sendResult.FailedTokens)
                    {
                        var subscribtionId = new SubscriptionId(failedToken.Token, tenant);
                        var unsubscribe = new UnSubscribe(subscribtionId, @event.SubscriberId, failedToken);
                        if (CommandPublisher.Publish(unsubscribe) == false)
                        {
                            log.Error("Unable to publish command" + Environment.NewLine + unsubscribe.ToString());
                        }
                    }
                }
            }

            try
            {
                BadgeCountTrackerFactory.GetService(tenant).Increment(subscriberId);  // We could issue a new command, but decided to remove added latency and execute via shortest route to DB
            }
            catch (Exception ex)
            {
                log.WarnException($"Could not Increment the badge counter for `{subscriberId}`", ex);
            }
        }

        private NotificationPayload SetNotificationPayloadBadgeCount(NotificationPayload payload, string tenant, string subscriberId)
        {
            try
            {
                StatCounter stat = BadgeCountTrackerFactory.GetService(tenant).Show(subscriberId);
                return new NotificationPayload(payload.Title, payload.Body, payload.Sound, payload.Icon, (int)stat.Count);
            }
            catch (Exception ex)
            {
                log.ErrorException("Unable to get badge counter", ex);
                return payload;
            }
        }

        public void Handle(TopicPushNotificationSent @event)
        {
            if (ReferenceEquals(null, Projections)) throw new ArgumentNullException(nameof(Projections));
            if (ReferenceEquals(null, DeliveryProvisioner)) throw new ArgumentNullException(nameof(DeliveryProvisioner));

            var topic = @event.Id.Topic;

            var notification = new NotificationForDelivery(@event.Id, @event.NotificationPayload, @event.NotificationData, @event.ExpiresAt, @event.ContentAvailable);

            var provisioners = DeliveryProvisioner.GetDeliveryProviders(@event.Id.Tenant);

            foreach (var provisioner in provisioners)
            {
                provisioner.SendToTopic(topic, notification);
            }
        }
    }
}
