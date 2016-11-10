using System;
using System.Linq;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.PushNotifications.Events;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using Projections;

namespace PushNotifications.Ports.APNS
{
    public class APNSPort : IPort,
        IEventHandler<PushNotificationWasSent>
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(APNSPort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IProjectionRepository Projections { get; set; }

        public IPushBroker PushBroker { get; set; }

        public void Handle(PushNotificationWasSent @event)
        {
            var subscriptions = Projections.LoadCollectionItems<APNSSubscriptionsProjection>(@event.UserId).Select(x => x.State);

            var disticntSubscriptions = subscriptions.Distinct(APNSSubscriptionsProjectionState.Comparer).ToList();

            if (disticntSubscriptions.Count == 0)
            {
                log.Debug("[APNS] Unable to find token for userId: '" + @event.UserId + "'");
                return;
            }

            foreach (var subscription in disticntSubscriptions)
            {
                try
                {
                    //Hard code this to 1 by request of Craig
                    //var badge = @event.Badge == 0 ? subscription.Badge + 1 : @event.Badge;
                    var badge = 1;
                    //This has to be implemented properlly
                    //subscription.Badge = badge;
                    //Projections.Query<APNSSubscriptionsProjection>().Save(subscription);
                    PushBroker.QueueNotification(BuildNotification(subscription.Token, @event.Json, @event.Text, badge, @event.Sound, @event.Category, @event.IsSilent));

                    log.Info("[APNS] Push notification '" + @event.Text + "' was queued using token '" + subscription.Token + "'");
                    log.Debug("[APNS] Push notification '" + @event.Text + "' was queued using token '" + subscription.Token + "'" + Environment.NewLine +
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
                            .WithContentAvailable(1)
                            .WithCustomItem("payload", json)
                            .WithSound(sound)
                            .WithBadge(badge)
                            .WithCategory(category);
            }

            return notification;
        }
    }
}
