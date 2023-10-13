using Elders.Cronus;
using PushNotifications.Subscriptions.Commands;
using System.Threading.Tasks;

namespace PushNotifications.Subscriptions
{
    public class DeviceSubscriptionAppService : ApplicationService<DeviceSubscription>,
        ICommandHandler<Subscribe>,
        ICommandHandler<UnSubscribe>
    {
        public DeviceSubscriptionAppService(IAggregateRepository repository) : base(repository) { }

        public async Task HandleAsync(Subscribe command)
        {
            ReadResult<DeviceSubscription> result = await repository.LoadAsync<DeviceSubscription>(command.Id).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                DeviceSubscription sub = result.Data;
                sub.Subscribe(command.SubscriberId);
                await repository.SaveAsync(sub).ConfigureAwait(false);
            }
            else if (result.NotFound)
            {
                DeviceSubscription sub = new DeviceSubscription(command.Id, command.SubscriberId, command.SubscriptionToken);
                await repository.SaveAsync(sub).ConfigureAwait(false);
            }
        }

        public async Task HandleAsync(UnSubscribe command)
        {
            ReadResult<DeviceSubscription> result = await repository.LoadAsync<DeviceSubscription>(command.Id).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                DeviceSubscription sub = result.Data;
                sub.UnSubscribe(command.SubscriberId);
                await repository.SaveAsync(sub).ConfigureAwait(false);
            }
        }
    }
}
