using System;
using System.Collections.Generic;
using PushNotifications.Contracts;
using PushNotifications.Delivery.FireBase.Logging;
using PushNotifications.Delivery.FireBase.Models;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("PushNotifications.Delivery.FireBase.Tests")]

namespace PushNotifications.Delivery.FireBase
{
    public partial class FireBaseDelivery
    {
        internal class ExpiredTokensDetector
        {
            internal static SendTokensResult GetNotRegisteredTokens(IList<SubscriptionToken> tokens, IList<FireBaseResponseModel.FireBaseResponseResultModel> result)
            {
                if (ReferenceEquals(null, result)) return SendTokensResult.Success;

                var sendPushNotificationResult = new List<SubscriptionToken>();

                for (int i = 0; i < result.Count; i++)
                {
                    var token = tokens[i];
                    if (string.Equals(result[i].Error, "NotRegistered", StringComparison.OrdinalIgnoreCase))
                    {
                        log.Info($"[FireBase] the token: '{token}' is not registered and will be removed from the subscriber");
                        sendPushNotificationResult.Add(token);
                    }
                }

                return new SendTokensResult(sendPushNotificationResult);
            }
        }
    }
}
