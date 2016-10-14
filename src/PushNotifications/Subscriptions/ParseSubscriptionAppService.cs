using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.Subscriptions.Commands;

namespace PushNotifications.Subscriptions
{
    public class ParseSubscriptionAppService : AggregateRootApplicationService<ParseSubscription>,
        ICommandHandler<SubscribeForParse>,
        ICommandHandler<UnSubscribeFromParse>
    {
        public void Handle(SubscribeForParse command)
        {
            ParseSubscription sub;

            if (Repository.TryLoad(command.Id, out sub))
            {
                Update(command.Id, x => x.Subscribe(command.UserId, command.Token));
            }
            else
            {
                sub = new ParseSubscription(command.Id, command.UserId, command.Token);

                Repository.Save<ParseSubscription>(sub);
            }
        }

        public void Handle(UnSubscribeFromParse command)
        {
            Update(command.Id, x => x.UnSubscribe(command.UserId, command.Token));
        }
    }
}