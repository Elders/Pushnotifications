using Elders.Cronus;
using PushNotifications.Contracts.Subscriptions.Commands;

namespace PushNotifications.Subscriptions
{
    public class TopicSubscriptionAppService : AggregateRootApplicationService<TopicSubscription>,
        ICommandHandler<SubscribeToTopic>,
        ICommandHandler<UnsubscribeFromTopic>
    {
        public void Handle(SubscribeToTopic command)
        {
            TopicSubscription topicSubscription;
            if (Repository.TryLoad(command.Id, out topicSubscription))
            {
                Update(command.Id, x => x.SubscribeToTopic(command.Id));
            }
            else
            {
                topicSubscription = new TopicSubscription(command.Id);
                Repository.Save<TopicSubscription>(topicSubscription);
            }
        }

        public void Handle(UnsubscribeFromTopic command)
        {
            TopicSubscription topicSubscription;
            if (Repository.TryLoad(command.Id, out topicSubscription))
            {
                Update(command.Id, x => x.UnsubscribeFromTopic(command.Id));
            }
        }
    }
}
