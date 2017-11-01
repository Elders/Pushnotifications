using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.PushNotifications.Commands;

namespace PushNotifications
{
    public class PushNotificationAppService : AggregateRootApplicationService<PushNotification>,
         ICommandHandler<SendPushNotification>
    {
        public void Handle(SendPushNotification command)
        {
            var notification = new PushNotification(command.Id, command.SubscriberId, command.NotificationPayload);
            Repository.Save(notification);
        }
    }
}
