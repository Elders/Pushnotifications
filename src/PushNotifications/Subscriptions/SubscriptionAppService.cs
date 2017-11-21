using Elders.Cronus.DomainModeling;
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
                Update(command.Id, x => x.Subscribe(command.SubscriberId, command.SubscriptionToken));
            }
            else
            {
                sub = new Subscription(command.Id, command.SubscriberId, command.SubscriptionToken, command.SubscriptionType);
                Repository.Save<Subscription>(sub);
            }
        }

        public void Handle(UnSubscribe command)
        {
            Update(command.Id, x => x.UnSubscribe(command.SubscriberId, command.SubscriptionToken));
        }
    }
}
