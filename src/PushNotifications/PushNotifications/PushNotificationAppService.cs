using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts;
using PushNotifications.Contracts.PushNotifications.Commands;

namespace PushNotifications
{
    public class PushNotificationAppService : AggregateRootApplicationService<PushNotification>,
         ICommandHandler<SendPushNotification>
    {
        public void Handle(SendPushNotification command)
        {
            // Ignore pushnotifications that expired
            if (Timestamp.UtcNow().DateTime > command.ExpiresAt.DateTime)
                return;

            var notification = new PushNotification(command.Id, command.SubscriberId, command.NotificationPayload, command.ExpiresAt, command.ContentAvailable);
            Repository.Save(notification);
        }
    }
}
