using Elders.Cronus;
using PushNotifications.Subscriptions.Commands;

namespace PushNotifications.Subscriptions
{
    public class TopicSubscriptionAppService : ApplicationService<TopicSubscription>,
        ICommandHandler<SubscribeToTopic>,
        ICommandHandler<UnsubscribeFromTopic>
    {
        public TopicSubscriptionAppService(IAggregateRepository repository) : base(repository) { }

        public void Handle(SubscribeToTopic command)
        {
            ReadResult<TopicSubscription> result = repository.Load<TopicSubscription>(command.Id);

            if (result.IsSuccess)
            {
                TopicSubscription topicSubscription = result.Data;
                topicSubscription.SubscribeToTopic(command.Id);
                repository.Save(topicSubscription);
            }
            else if (result.NotFound)
            {
                TopicSubscription topicSubscription = new TopicSubscription(command.Id);
                repository.Save(topicSubscription);
            }
        }

        public void Handle(UnsubscribeFromTopic command)
        {
            ReadResult<TopicSubscription> result = repository.Load<TopicSubscription>(command.Id);
            if (result.IsSuccess)
            {
                TopicSubscription topicSubscription = result.Data;
                topicSubscription.UnsubscribeFromTopic(command.Id);
                repository.Save(topicSubscription);
            }
        }
    }
}
