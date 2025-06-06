﻿using PushNotifications.Contracts.PushNotifications.Delivery;
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

    public async Task<SubscribeUnsubscribeResult> SubscribeToTopicAsync(SubscriptionToken token, Topic topic)
    {
        var result = await pushyClient.SubscribeToTopicAsync(token, topic).ConfigureAwait(false);
        if (result == true)
        {
            return SubscribeUnsubscribeResult.Successful();
        }
        return SubscribeUnsubscribeResult.Unsuccessful("There is an error while subscribing user for topic in pushy");
    }
    public async Task<SubscribeUnsubscribeResult> UnsubscribeFromTopicAsync(SubscriptionToken token, Topic topic)
    {
        bool unsubscribeResult = await pushyClient.UnsubscribeFromTopicAsync(token, topic).ConfigureAwait(false);
        if (unsubscribeResult == true)
        {
            return SubscribeUnsubscribeResult.Successful();
        }
        return SubscribeUnsubscribeResult.Unsuccessful("There is an error while unsubscribing user from topic in pushy");
    }
}
