using System.Linq;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts.PushNotifications.Events;
using PushSharp;
using PushSharp.Apple;

namespace PushNotifications.Ports.APNS
{
    public class APNSPort : IPort, IPushNotificationPort, IHaveProjectionsRepository,
        IEventHandler<PushNotificationWasSent>
    {
        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IRepository Repository { get; set; }

        public PushBroker PushBroker { get; set; }

        public void Handle(PushNotificationWasSent @event)
        {
            var tokens = Repository.Query<APNSSubscriptions>().GetCollection(@event.UserId);

            foreach (var token in tokens.Select(x => x.Token).Distinct().ToList())
            {
                PushBroker.QueueNotification(BuildNotification(token, @event.Json, @event.Text, @event.Badge, @event.Sound, @event.Category, @event.IsSilent));
            }
        }

        private AppleNotification BuildNotification(string token, string json, string text, int badge, string sound, string category, bool isSilent)
        {
            if (string.IsNullOrWhiteSpace(sound))
                sound = "default";

            if (json == null)
                json = string.Empty;

            if (text == null)
                text = string.Empty;

            var notification = new AppleNotification();

            if (isSilent)
            {
                notification.ForDeviceToken(token)
                            .WithAlert(string.Empty)
                            .WithContentAvailable(1)
                            .WithCustomItem("payload", json)
                            .WithCategory(category);
            }
            else
            {
                notification.ForDeviceToken(token)
                            .WithAlert(text)
                            .WithCustomItem("payload", json)
                            .WithSound(sound)
                            .WithBadge(badge)
                            .WithCategory(category);
            }

            return notification;
        }
    }
}
