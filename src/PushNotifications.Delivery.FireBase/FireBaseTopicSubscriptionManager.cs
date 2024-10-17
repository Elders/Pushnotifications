using Elders.Cronus.MessageProcessing;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery.FireBase;
using PushNotifications.Subscriptions;
using System.Collections.Generic;
using System.Threading.Tasks;

public sealed class FireBaseTopicSubscriptionManager : ITopicSubscriptionManager
{
    private readonly FirebaseAppOptionsContainer firebaseAppOptionsContainer;
    private readonly ICronusContextAccessor cronusContextAccessor;

    public FireBaseTopicSubscriptionManager(FirebaseAppOptionsContainer firebaseAppOptionsContainer, ICronusContextAccessor cronusContextAccessor)
    {
        this.firebaseAppOptionsContainer = firebaseAppOptionsContainer;
        this.cronusContextAccessor = cronusContextAccessor;
    }

    public SubscriptionType Platform => SubscriptionType.FireBase;

    public async Task<bool> SubscribeToTopicAsync(SubscriptionToken token, Topic topic)
    {
        FirebaseMessaging client = GetMessagingClient("vapt"); // TODO: get application from topic or token

        TopicManagementResponse subscribeResult = await client.SubscribeToTopicAsync(new List<string> { token.Token }, topic).ConfigureAwait(false);

        // TODO: Add better error handling
        // We send a list of tokens with single token, so we expect a success count of 1
        if (subscribeResult.SuccessCount != 1)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> UnsubscribeFromTopicAsync(SubscriptionToken token, Topic topic)
    {
        FirebaseMessaging client = GetMessagingClient("vapt"); // TODO: get application from topic or token

        TopicManagementResponse unsubscribeResult = await client.SubscribeToTopicAsync(new List<string> { token.Token }, topic).ConfigureAwait(false);

        // TODO: Add better error handling
        // We send a list of tokens with single token, so we expect a success count of 1
        if (unsubscribeResult.SuccessCount != 1)
        {
            return false;
        }

        return true;
    }

    private FirebaseMessaging GetMessagingClient(string application)
    {
        FirebaseApp app = FirebaseApp.GetInstance(application);
        if (app is null)
        {
            AppOptions appOptions = firebaseAppOptionsContainer.GetAppOptions(cronusContextAccessor.CronusContext.Tenant, application);
            lock (stupidFirebaseLock)
            {
                app = FirebaseApp.GetInstance(application);
                if (app is null)
                    app = FirebaseApp.Create(appOptions, application);
            }
        }
        return FirebaseMessaging.GetMessaging(app);
    }

    private object stupidFirebaseLock = new object();
}
