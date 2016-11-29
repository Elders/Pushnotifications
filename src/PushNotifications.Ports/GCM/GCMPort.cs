using System;
using System.Linq;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.PushNotifications.Events;
using PushSharp;
using PushSharp.Android;
using PushSharp.Core;
using Projections;

namespace PushNotifications.Ports.GCM
{
    public class GCMPort : IPort,
        IEventHandler<PushNotificationWasSent>
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(GCMPort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IProjectionRepository Projections { get; set; }

        public IPushBroker PushBroker { get; set; }

        public void Handle(PushNotificationWasSent @event)
        {
            var tokens = Projections.LoadCollectionItems<GCMSubscriptionsProjection>(@event.UserId).Where(x => ReferenceEquals(x, null) == false).Select(x => x.State);

            if (ReferenceEquals(tokens, null) == true || tokens.Any() == false)
                return;

            var distinctTokens = tokens.Select(x => x.Token).Distinct().ToList();

            if (distinctTokens.Count == 0)
            {
                log.Debug("[GCM] Unable to find token for userId: '" + @event.UserId + "'");

                return;
            }

            foreach (var token in distinctTokens)
            {
                try
                {
                    PushBroker.QueueNotification(new GcmNotification()
                        .ForDeviceRegistrationId(token)
                        .WithJson(@event.Json));

                    log.Info("[GCM] Push notification '" + @event.Text + "' was queued using token '" + token + "'");
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }
    }
}
