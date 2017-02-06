using System;
using System.Linq;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.PushNotifications.Events;
using PushSharp.Core;
using Projections;
using System.Collections.Generic;

namespace PushNotifications.Ports.Pushy
{
    public class PushyPort : IPort,
        IEventHandler<PushNotificationWasSent>
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(PushyPort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IProjectionRepository Projections { get; set; }

        public IPushBroker PushBroker { get; set; }

        public void Handle(PushNotificationWasSent @event)
        {
            var tokens = Projections.LoadCollectionItems<PushySubscriptionsProjection>(@event.UserId).Where(x => ReferenceEquals(x, null) == false).Select(x => x.State);

            if (ReferenceEquals(tokens, null) == true || tokens.Any() == false)
                return;

            var distinctTokens = tokens.Select(x => x.Token).Distinct().ToList();

            if (distinctTokens.Count == 0)
            {
                log.Debug("[Pushy] Unable to find token for userId: '" + @event.UserId + "'");

                return;
            }

            foreach (var token in distinctTokens)
            {
                try
                {
                    PushBroker.QueueNotification(new PushNotifications.Pushy.PushyNotification(@event.Json, new List<string> { token }));

                    log.Info("[Pushy] Push notification '" + @event.Text + "' was queued using token '" + token + "'");
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }
    }
}
