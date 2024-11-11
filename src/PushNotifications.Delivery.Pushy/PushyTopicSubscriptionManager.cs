using Elders.Cronus.Userfull;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery;
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

    public async Task<SubscribeUnsubscribeHandler> SubscribeToTopicAsync(SubscriptionToken token, Topic topic)
    {
        var result = await pushyClient.SubscribeToTopicAsync(token, topic).ConfigureAwait(false);
        if (result == true)
        {
            return SubscribeUnsubscribeHandler.Successful();
        }
        return SubscribeUnsubscribeHandler.Unsuccessful(null);
    }
    public async Task<SubscribeUnsubscribeHandler> UnsubscribeFromTopicAsync(SubscriptionToken token, Topic topic)
    {
        bool unsubscribeResult = await pushyClient.UnsubscribeFromTopicAsync(token, topic).ConfigureAwait(false);
        if (unsubscribeResult == true)
        {
            return SubscribeUnsubscribeHandler.Successful();
        }
        return SubscribeUnsubscribeHandler.Unsuccessful(null);
    }
}
