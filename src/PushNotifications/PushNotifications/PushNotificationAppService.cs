using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.PushNotifications.Commands;

namespace PushNotifications.PushNotifications
{
    public class PushNotificationAppService : AggregateRootApplicationService<PushNotification>,
        ICommandHandler<SendPushNotification>
    {
        public void Handle(SendPushNotification command)
        {
            var notification = new PushNotification(command.Id, command.UserId, command.Json, command.Text, command.Sound, command.Icon, command.Category, command.Badge, command.IsSilent);

            Repository.Save<PushNotification>(notification);
        }
    }
}