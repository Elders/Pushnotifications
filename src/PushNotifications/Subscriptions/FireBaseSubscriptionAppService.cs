using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.FireBaseSubscriptions.Commands;

namespace PushNotifications.Subscriptions
{
    public class FireBaseSubscriptionAppService : AggregateRootApplicationService<FireBaseSubscription>,
        ICommandHandler<SubscribeUserForFireBase>,
        ICommandHandler<UnSubscribeUserFromFireBase>
    {
        public void Handle(SubscribeUserForFireBase command)
        {
            FireBaseSubscription sub;
            if (Repository.TryLoad(command.Id, out sub))
            {
                Update(command.Id, x => x.Subscribe(command.UserId, command.Token));
            }
            else
            {
                sub = new FireBaseSubscription(command.Id, command.UserId, command.Token);
                Repository.Save<FireBaseSubscription>(sub);
            }
        }

        public void Handle(UnSubscribeUserFromFireBase command)
        {
            Update(command.Id, x => x.UnSubscribe(command.UserId, command.Token));
        }
    }
}
