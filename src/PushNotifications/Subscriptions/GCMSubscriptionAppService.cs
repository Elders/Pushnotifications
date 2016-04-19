using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.Subscriptions.Commands;

namespace PushNotifications.Subscriptions
{
    public class GCMSubscriptionAppService : AggregateRootApplicationService<GCMSubscription>,
        ICommandHandler<SubscribeForGCM>,
        ICommandHandler<UnSubscribeFromGCM>
    {
        public void Handle(SubscribeForGCM command)
        {
            GCMSubscription sub;

            if (Repository.TryLoad(command.Id, out sub))
            {
                Update(command.Id, x => x.Subscribe(command.UserId, command.Token));
            }
            else
            {
                sub = new GCMSubscription(command.Id, command.UserId, command.Token);

                Repository.Save<GCMSubscription>(sub);
            }
        }

        public void Handle(UnSubscribeFromGCM command)
        {
            Update(command.Id, x => x.UnSubscribe(command.UserId, command.Token));
        }
    }
}