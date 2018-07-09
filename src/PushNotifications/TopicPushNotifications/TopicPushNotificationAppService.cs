using Elders.Cronus;
using PushNotifications.Contracts.PushNotifications.Commands;

namespace PushNotifications
{
    public class TopicPushNotificationAppService : AggregateRootApplicationService<TopicPushNotification>,
         ICommandHandler<TopicSendPushNotification>
    {
        public void Handle(TopicSendPushNotification command)
        {
            var notification = new TopicPushNotification(command.Id, command.Topic, command.NotificationPayload, command.NotificationData, command.ExpiresAt, command.ContentAvailable);
            Repository.Save(notification);
        }
    }
}
