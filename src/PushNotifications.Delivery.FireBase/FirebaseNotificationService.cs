using Elders.Cronus;
using Elders.Cronus.MessageProcessing;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using PushNotifications.Contracts.PushNotifications.Delivery;
using PushNotifications.PushNotifications;
using PushNotifications.Subscriptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PushNotifications.Delivery.FireBase
{
    public sealed class FirebaseNotificationService
    {
        private readonly FirebaseAppOptionsContainer firebaseAppOptionsContainer;
        private readonly ICronusContextAccessor cronusContextAccessor;
        private readonly ILogger<FirebaseNotificationService> logger;

        public FirebaseNotificationService(FirebaseAppOptionsContainer firebaseAppOptionsContainer, ILogger<FirebaseNotificationService> logger, ICronusContextAccessor cronusContextAccessor)
        {
            this.firebaseAppOptionsContainer = firebaseAppOptionsContainer;
            this.logger = logger;
            this.cronusContextAccessor = cronusContextAccessor;
        }

        public async Task<SendTokensResult> SendNotificationsAsync(IEnumerable<SubscriptionToken> subTokens, NotificationForDelivery notification)
        {
            FirebaseMessaging client = GetMessagingClient(notification.Target.Application);

            string badge = notification.NotificationPayload.Badge > 0 ? notification.NotificationPayload.Badge.ToString() : "1";

            SendTokensResult finalResult = new SendTokensResult(new List<SubscriptionToken>());
            int skip = 0;
            int take = 1000;

            while (true)
            {
                List<string> tokenBatch = subTokens.Skip(skip).Take(take).Select(x => x.Token).ToList();
                if (tokenBatch.Count > 0)
                {
                    MulticastMessage message = new MulticastMessage()
                    {
                        Tokens = tokenBatch,
                        Data = notification.NotificationData.ToDictionary(x => x.Key, y => y.Value.ToString()),
                        Notification = new Notification()
                        {
                            Title = notification.NotificationPayload.Title,
                            Body = notification.NotificationPayload.Body
                        }
                    };

                    BatchResponse response = await client.SendEachForMulticastAsync(message);

                    skip += take;

                    if (response.SuccessCount == 0)
                        return SendTokensResult.Failed;

                    if (response.FailureCount == tokenBatch.Count)
                        return SendTokensResult.Failed;

                    if (response.FailureCount > 0)
                    {
                        List<string> failedTokens = new List<string>();
                        for (var i = 0; i < response.Responses.Count; i++)
                        {
                            if (!response.Responses[i].IsSuccess)
                            {
                                // The order of responses corresponds to the order of the registration tokens.
                                failedTokens.Add(tokenBatch[i]);
                            }
                        }

                        logger.Error(() => $"Failed tokens during PN sending for {notification.Target}: {string.Join(", ", failedTokens)}");
                    }
                }
                else
                {
                    break;
                }
            }

            return finalResult;
        }


        public FirebaseMessaging GetMessagingClient(string application)
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
}
