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
                Update(command.Id, x => x.SubscribeToTopic(command.Id, command.SubscriberId, command.Topic, command.SubscriptionType));
            }
            else
            {
                topicSubscription = new TopicSubscription(command.Id, command.SubscriberId, command.Topic, command.SubscriptionType);
                Repository.Save<TopicSubscription>(topicSubscription);
            }
        }

        public void Handle(UnsubscribeFromTopic command)
        {
            TopicSubscription topicSubscription;
            if (Repository.TryLoad(command.Id, out topicSubscription))
            {
                Update(command.Id, x => x.UnsubscribeFromTopic(command.Id, command.SubscriberId, command.Topic, command.SubscriptionType));
            }
        }
    }
}
