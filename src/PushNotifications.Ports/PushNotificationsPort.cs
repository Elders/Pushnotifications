using System;
using Elders.Cronus;
using Elders.Cronus.Projections;
using Multitenancy.Delivery;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.PushNotifications.Events;
using PushNotifications.Ports.Logging;
using PushNotifications.Projections.Subscriptions;

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

        public void Handle(PushNotificationSent @event)
        {
            if (ReferenceEquals(null, Projections)) throw new ArgumentNullException(nameof(Projections));
            if (ReferenceEquals(null, DeliveryProvisioner)) throw new ArgumentNullException(nameof(DeliveryProvisioner));

            var projectionReponse = Projections.Get<SubscriberTokensProjection>(@event.SubscriberId);
            if (projectionReponse.Success == false)
            {
                log.Info(() => $"No tokens were found for subscriber {@event.SubscriberId}");
                return;
            }

            foreach (var token in projectionReponse.Projection.State.Tokens)
            {
                var notification = new NotificationForDelivery(@event.Id, @event.NotificationPayload, @event.NotificationData, @event.ExpiresAt, @event.ContentAvailable);
                var delivery = DeliveryProvisioner.ResolveDelivery(token.SubscriptionType, notification);
                delivery.Send(token, notification);
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
