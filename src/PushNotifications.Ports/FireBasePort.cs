using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts.FireBaseSubscriptions;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Contracts.PushNotifications.Events;
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

        public IPushNotificationDelivery Delivery { get; set; }

        public void Handle(PushNotificationSent @event)
        {
            var projectionReponse = Projections.Get<SubscriberTokensForFireBaseProjection>(@event.SubscriberId);
            if (projectionReponse.Success == false)
                return;

            foreach (var token in projectionReponse.Projection.State.Tokens)
            {
                var notification = new FireBaseNotificationDelivery(@event.Id, @event.NotificationPayload, @event.ExpiresAt, @event.ContentAvailable);
                Delivery.Send(token, notification);
            }
        }
    }
}
