using System.Linq;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts.PushNotifications.Events;
using PushSharp;

namespace PushNotifications.Ports.Parse
{
    public class ParsePort : IPort, IPushNotificationPort, IHaveProjectionsRepository,
        IEventHandler<PushNotificationWasSent>
    {
        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IRepository Repository { get; set; }

        public PushBroker PushBroker { get; set; }

        public void Handle(PushNotificationWasSent @event)
        {
            var tokens = Repository.Query<ParseSubscriptions>().GetCollection(@event.UserId);

            foreach (var token in tokens.Select(x => x.Token).Distinct().ToList())
            {
                PushBroker.QueueNotification(new ParseAndroidNotifcation(token, @event.Json));
            }
        }
    }
}
