using System;
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
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(APNSPort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IRepository Repository { get; set; }

        public PushBroker PushBroker { get; set; }

        public void Handle(PushNotificationWasSent @event)
        {
            var tokens = Repository.Query<APNSSubscriptions>().GetCollection(@event.UserId);

            var distinctTokens = tokens.Select(x => x.Token).Distinct().ToList();

            if (distinctTokens.Count == 0)
            {
                log.Error("[APNS] Unable to find token for userId: '" + @event.UserId + "'");
                return;
            }

            foreach (var token in distinctTokens)
            {
                try
                {
                    PushBroker.QueueNotification(BuildNotification(token, @event.Json, @event.Text, @event.Badge, @event.Sound, @event.Category, @event.IsSilent));

                    log.Info("[APNS] Push notification '" + @event.Text + "' was queued using token '" + token + "'");
                    log.Debug("[APNS] Push notification '" + @event.Text + "' was queued using token '" + token + "'" + Environment.NewLine +
                        @event.Json + Environment.NewLine +
                        @event.Badge + Environment.NewLine +
                        @event.Sound + Environment.NewLine +
                        @event.Category + Environment.NewLine +
                        @event.IsSilent);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
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
