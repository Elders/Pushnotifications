using System;
using System.Linq;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts.PushNotifications.Events;
using PushSharp;
using PushSharp.Core;

namespace PushNotifications.Ports.Parse
{
    public class ParsePort : IPort, IPushNotificationPort, IHaveProjectionsRepository,
        IEventHandler<PushNotificationWasSent>
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ParsePort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IRepository Repository { get; set; }

        public IPushBroker PushBroker { get; set; }

        public void Handle(PushNotificationWasSent @event)
        {
            var tokens = Repository.Query<ParseSubscriptions>().GetCollection(@event.UserId);

            var distinctTokens = tokens.Select(x => x.Token).Distinct().ToList();

            if (distinctTokens.Count == 0)
            {
                log.Debug("[PARSE] Unable to find token for userId: '" + @event.UserId + "'");

                return;
            }

            foreach (var token in distinctTokens)
            {
                try
                {
                    PushBroker.QueueNotification(new ParseAndroidNotifcation(token, @event.Json));

                    log.Info("[PARSE] Push notification '" + @event.Text + "' was queued using token '" + token + "'");
                    log.Debug("[PARSE] Push notification '" + @event.Text + "' was queued using token '" + token + "'" + Environment.NewLine +
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
    }
}
