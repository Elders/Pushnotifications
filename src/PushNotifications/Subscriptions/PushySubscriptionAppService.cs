using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.Subscriptions.Commands;

namespace PushNotifications.Subscriptions
{
    public class PushySubscriptionAppService : AggregateRootApplicationService<PushySubscription>,
        ICommandHandler<SubscribeForPushy>,
        ICommandHandler<UnSubscribeFromPushy>
    {
        public void Handle(SubscribeForPushy command)
        {
            PushySubscription sub;

            if (Repository.TryLoad(command.Id, out sub))
            {
                Update(command.Id, x => x.Subscribe(command.UserId, command.Token));
            }
            else
            {
                sub = new PushySubscription(command.Id, command.UserId, command.Token);

                Repository.Save<PushySubscription>(sub);
            }
        }

        public void Handle(UnSubscribeFromPushy command)
        {
            Update(command.Id, x => x.UnSubscribe(command.UserId, command.Token));
        }
    }
}
