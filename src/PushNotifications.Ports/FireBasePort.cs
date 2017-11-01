using System;
using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.PushNotifications.Events;
using PushNotifications.Ports.Logging;

namespace PushNotifications.Ports
{
    public class FireBasePort : IPort,
        IEventHandler<PushNotificationSent>
    {
        static ILog log = LogProvider.GetLogger(typeof(FireBasePort));

        public IPublisher<ICommand> CommandPublisher { get; set; }

        public void Handle(PushNotificationSent @event)
        {
            //throw new NotImplementedException();
        }
    }
}
