using Elders.Cronus;
using PushNotifications.Subscriptions.Commands;
using System.Threading.Tasks;

namespace PushNotifications.Subscriptions
{
    public class TopicSubscriptionAppService : ApplicationService<TopicSubscription>,
        ICommandHandler<SubscribeToTopic>,
        ICommandHandler<UnsubscribeFromTopic>
    {
        public TopicSubscriptionAppService(IAggregateRepository repository) : base(repository) { }

        public async Task HandleAsync(SubscribeToTopic command)
        {
            ReadResult<TopicSubscription> result = await repository.LoadAsync<TopicSubscription>(command.Id).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                TopicSubscription topicSubscription = result.Data;
                topicSubscription.SubscribeToTopic(command.Id, command.Timestamp);
                await repository.SaveAsync(topicSubscription).ConfigureAwait(false);
            }
            else if (result.NotFound)
            {
                TopicSubscription topicSubscription = new TopicSubscription(command.Id);
                await repository.SaveAsync(topicSubscription).ConfigureAwait(false);
            }
        }

        public async Task HandleAsync(UnsubscribeFromTopic command)
        {
            ReadResult<TopicSubscription> result = await repository.LoadAsync<TopicSubscription>(command.Id).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                TopicSubscription topicSubscription = result.Data;
                topicSubscription.UnsubscribeFromTopic(command.Id, command.Timestamp);
                await repository.SaveAsync(topicSubscription).ConfigureAwait(false);
            }
        }
    }
}
