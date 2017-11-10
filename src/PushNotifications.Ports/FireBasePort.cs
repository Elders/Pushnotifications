using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.PushNotifications.Events;
using PushNotifications.Delivery.Bulk;
using PushNotifications.Delivery.FireBase;
using PushNotifications.Ports.Logging;
using PushNotifications.Projections.FireBase;

namespace PushNotifications.Ports
{
    public class FireBasePort : IPort,
        IEventHandler<PushNotificationSent>
    {
        static ILog log = LogProvider.GetLogger(typeof(FireBasePort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IProjectionRepository Projections { get; set; }

        public BulkDelivery<FireBaseDelivery> FireBaseDelivery { get; set; }

        public void Handle(PushNotificationSent @event)
        {
            var projectionReponse = Projections.Get<SubscriberTokensForFireBaseProjection>(@event.SubscriberId);
            if (projectionReponse.Success == false)
                return;

            foreach (var token in projectionReponse.Projection.State.Tokens)
            {
                var notification = new NotificationDelivery(@event.NotificationPayload, @event.ExpiresAt, @event.ContentAvailable);
                FireBaseDelivery.Send(token, notification);
            }
        }
    }
}
