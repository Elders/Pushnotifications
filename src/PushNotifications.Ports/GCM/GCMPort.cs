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
        log4net.ILog log = log4net.LogManager.GetLogger(typeof(GCMPort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public IRepository Repository { get; set; }

        public PushBroker PushBroker { get; set; }

        public void Handle(PushNotificationWasSent @event)
        {
            var tokens = Repository.Query<GCMSubscriptions>().GetCollection(@event.UserId);

            foreach (var token in tokens.Select(x => x.Token).Distinct().ToList())
            {
                try
                {
                    PushBroker.QueueNotification(new GcmNotification()
                        .ForDeviceRegistrationId(token)
                        .WithJson(@event.Json));

                    log.Info("Android push notification was sent to device with token: " + token);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }
    }
}
