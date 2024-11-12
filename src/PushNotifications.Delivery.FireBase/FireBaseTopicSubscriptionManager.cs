using Elders.Cronus.MessageProcessing;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.Delivery.FireBase;
using PushNotifications.Subscriptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using PushNotifications.Delivery;

namespace PushNotifications.Ports;
public sealed class FireBaseTopicSubscriptionManager : ITopicSubscriptionManager
{
    private readonly FirebaseAppOptionsContainer firebaseAppOptionsContainer;
    private readonly ICronusContextAccessor cronusContextAccessor;
    private readonly ILogger<FireBaseTopicSubscriptionManager> logger;

    public FireBaseTopicSubscriptionManager(FirebaseAppOptionsContainer firebaseAppOptionsContainer, ICronusContextAccessor cronusContextAccessor, ILogger<FireBaseTopicSubscriptionManager> logger)
    {
        this.firebaseAppOptionsContainer = firebaseAppOptionsContainer;
        this.cronusContextAccessor = cronusContextAccessor;
        this.logger = logger;
    }

    public SubscriptionType Platform => SubscriptionType.FireBase;

    public async Task<SubscribeUnsubscribeResult> SubscribeToTopicAsync(SubscriptionToken token, Topic topic)
    {
        FirebaseMessaging client = GetMessagingClient("vapt"); // TODO: get application from topic or token

        TopicManagementResponse subscribeResult = await client.SubscribeToTopicAsync(new List<string> { token.Token }, topic).ConfigureAwait(false);

        // TODO: Add better error handling
        // We send a list of tokens with single token, so we expect a success count of 1
        if (subscribeResult.SuccessCount != 1)
        {
            string error = string.Join(", ", subscribeResult.Errors.Select(x => x.Reason));
            logger.LogError("There was an error while subscribing token for topic. ERROR: {error}", error);

            return SubscribeUnsubscribeResult.Unsuccessful(subscribeResult.Errors.Select(x => x.Reason));
        }

        return SubscribeUnsubscribeResult.Successful();
    }

    public async Task<SubscribeUnsubscribeResult> UnsubscribeFromTopicAsync(SubscriptionToken token, Topic topic)
    {
        FirebaseMessaging client = GetMessagingClient("vapt"); // TODO: get application from topic or token

        TopicManagementResponse unsubscribeResult = await client.UnsubscribeFromTopicAsync(new List<string> { token.Token }, topic).ConfigureAwait(false);

        // TODO: Add better error handling
        // We send a list of tokens with single token, so we expect a success count of 1
        if (unsubscribeResult.SuccessCount != 1)
        {
            string error = string.Join(", ", unsubscribeResult.Errors.Select(x => x.Reason));
            logger.LogError("There was an error while unsubscribing token for topic. ERROR: {error}", error);

            return SubscribeUnsubscribeResult.Unsuccessful(unsubscribeResult.Errors.Select(x => x.Reason));
        }

        return SubscribeUnsubscribeResult.Successful();
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
