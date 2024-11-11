using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery.Pushy;
using PushNotifications.Subscriptions;
using System.Threading.Tasks;

public sealed class PushyTopicSubscriptionManager : ITopicSubscriptionManager
{
    private readonly PushyClient pushyClient;

    public PushyTopicSubscriptionManager(PushyClient pushyClient)
    {
        this.pushyClient = pushyClient;
    }

    public SubscriptionType Platform => SubscriptionType.Pushy;

    public async Task<bool> SubscribeToTopicAsync(SubscriptionToken token, Topic topic)
    {
        bool subscribeResult = await pushyClient.SubscribeToTopicAsync(token, topic).ConfigureAwait(false);

        return subscribeResult;
    }

    public Task<object> TrySubscribeToTopicAsync(SubscriptionToken token, Topic topic)
    {
        throw new System.NotImplementedException();
    }

    public Task<object> TryUnsubscribeFromTopicAsync(SubscriptionToken token, Topic topic)
    {
        throw new System.NotImplementedException();
    }

    public async Task<bool> UnsubscribeFromTopicAsync(SubscriptionToken token, Topic topic)
    {
        bool unsubscribeResult = await pushyClient.UnsubscribeFromTopicAsync(token, topic).ConfigureAwait(false);

        return unsubscribeResult;
    }
}
