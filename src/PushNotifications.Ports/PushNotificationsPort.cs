using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using Multitenancy.Delivery;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.PushNotifications.Events;
using PushNotifications.Ports.Logging;
using PushNotifications.Projections.Subscriptions;

namespace PushNotifications.Ports
{
    public class PushNotificationsPort : IPort,
        IEventHandler<PushNotificationSent>
    {
        static ILog log = LogProvider.GetLogger(typeof(PushNotificationsPort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IProjectionRepository Projections { get; set; }

        public IDeliveryProvisioner DeliveryProvisioner { get; set; }

        public void Handle(PushNotificationSent @event)
        {
            var projectionReponse = Projections.Get<SubscriberTokensProjection>(@event.SubscriberId);
            if (projectionReponse.Success == false)
                return;

            foreach (var token in projectionReponse.Projection.State.Tokens)
            {
                var notification = new NotificationForDelivery(@event.Id, @event.NotificationPayload, @event.ExpiresAt, @event.ContentAvailable);
                var delivery = DeliveryProvisioner.ResolveDelivery(token.SubscriptionType, notification);
                delivery.Send(token, notification);
            }
        }
    }
}
