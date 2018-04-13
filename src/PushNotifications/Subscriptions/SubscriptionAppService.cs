using Elders.Cronus;
using PushNotifications.Contracts.Subscriptions.Commands;

namespace PushNotifications.Subscriptions
{
    public class SubscriptionAppService : AggregateRootApplicationService<Subscription>,
        ICommandHandler<Subscribe>,
        ICommandHandler<UnSubscribe>
    {
        public void Handle(Subscribe command)
        {
            Subscription sub;
            if (Repository.TryLoad(command.Id, out sub))
            {
                Update(command.Id, x => x.Subscribe(command.SubscriberId));
            }
            else
            {
                sub = new Subscription(command.Id, command.SubscriberId, command.SubscriptionToken);
                Repository.Save<Subscription>(sub);
            }
        }

        public void Handle(UnSubscribe command)
        {
            Subscription sub;
            if (Repository.TryLoad(command.Id, out sub))
            {
                Update(command.Id, x => x.UnSubscribe(command.SubscriberId));
            }
        }
    }
}
