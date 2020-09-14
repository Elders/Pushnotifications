using Elders.Cronus;
using PushNotifications.Subscriptions.Commands;

namespace PushNotifications.Subscriptions
{
    public class SubscriptionAppService : ApplicationService<Subscription>,
        ICommandHandler<Subscribe>,
        ICommandHandler<UnSubscribe>
    {
        public SubscriptionAppService(IAggregateRepository repository) : base(repository) { }

        public void Handle(Subscribe command)
        {
            ReadResult<Subscription> result = repository.Load<Subscription>(command.Id);
            if (result.IsSuccess)
            {
                Subscription sub = result.Data;
                sub.Subscribe(command.SubscriberId);
                repository.Save(sub);
            }
            else if (result.NotFound)
            {
                Subscription sub = new Subscription(command.Id, command.SubscriberId, command.SubscriptionToken);
                repository.Save(sub);
            }
        }

        public void Handle(UnSubscribe command)
        {
            ReadResult<Subscription> result = repository.Load<Subscription>(command.Id);
            if (result.IsSuccess)
            {
                Subscription sub = result.Data;
                sub.UnSubscribe(command.SubscriberId);
                repository.Save(sub);
            }
        }
    }
}
