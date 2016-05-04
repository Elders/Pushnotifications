using System;
using System.Linq;
using Elders.Cronus.DomainModeling;
using Elders.Cronus.DomainModeling.Projections;
using PushNotifications.Contracts.PushNotifications.Events;
using PushSharp;
using PushSharp.Android;

namespace PushNotifications.Ports.GCM
{
    public class GCMPort : IPort, IPushNotificationPort, IHaveProjectionsRepository,
        IEventHandler<PushNotificationWasSent>
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(GCMPort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IRepository Repository { get; set; }

        public PushBroker PushBroker { get; set; }

        public void Handle(PushNotificationWasSent @event)
        {
            var tokens = Repository.Query<GCMSubscriptions>().GetCollection(@event.UserId);

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
