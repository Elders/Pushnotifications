using Elders.Cronus;
using PushNotifications.Subscriptions.Commands;

namespace PushNotifications.Subscriptions
{
    public class DeviceSubscriptionAppService : ApplicationService<DeviceSubscription>,
        ICommandHandler<Subscribe>,
        ICommandHandler<UnSubscribe>
    {
        public DeviceSubscriptionAppService(IAggregateRepository repository) : base(repository) { }

        public void Handle(Subscribe command)
        {
            ReadResult<DeviceSubscription> result = repository.Load<DeviceSubscription>(command.Id);
            if (result.IsSuccess)
            {
                DeviceSubscription sub = result.Data;
                sub.Subscribe(command.SubscriberId);
                repository.Save(sub);
            }
            else if (result.NotFound)
            {
                DeviceSubscription sub = new DeviceSubscription(command.Id, command.SubscriberId, command.SubscriptionToken);
                repository.Save(sub);
            }
        }

        public void Handle(UnSubscribe command)
        {
            ReadResult<DeviceSubscription> result = repository.Load<DeviceSubscription>(command.Id);
            if (result.IsSuccess)
            {
                DeviceSubscription sub = result.Data;
                sub.UnSubscribe(command.SubscriberId);
                repository.Save(sub);
            }
        }
    }
}
