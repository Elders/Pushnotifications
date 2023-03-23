using PushNotifications.PushNotifications;
using PushNotifications.Subscriptions;
using System;
using System.Collections.Generic;

namespace PushNotifications.Delivery.FireBase.Models
{
    public class FireBaseResponseModel
    {
        public int Success { get; set; }

        public int Failure { get; set; }

        public List<FireBaseResponseResultModel> Results { get; set; }

        public class FireBaseResponseResultModel
        {
            /// <summary>
            /// An existing registration token may cease to be valid in a number of scenarios, including:
            ///     * If the client app unregisters with FCM.
            ///     * If the client app is automatically unregistered, which can happen if the user uninstalls the application. For example, on iOS, if the APNS Feedback Service reported the APNS token as invalid.
            ///     * If the registration token expires (for example, Google might decide to refresh registration tokens, or the APNS token has expired for iOS devices).
            ///     * If the client app is updated but the new version is not configured to receive messages.
            ///For all these cases, remove this registration token from the app server and stop using it to send messages.
            /// </summary>
            public const string UnregisteredDevice = "NotRegistered";

            public string Error { get; set; }
        }

        internal class ExpiredTokensDetector
        {
            internal static SendTokensResult GetNotRegisteredTokens(IList<SubscriptionToken> tokens, IList<FireBaseResponseResultModel> responseModel)
            {
                if (ReferenceEquals(null, tokens)) throw new ArgumentNullException(nameof(tokens));
                if (ReferenceEquals(null, responseModel)) throw new ArgumentNullException(nameof(responseModel));

                if (tokens.Count != responseModel.Count)
                {
                    //log.Error(() =>
                    //{
                    //    string tokensAsString = string.Join(",", tokens);
                    //    string responseModelAsString = string.Join(",", responseModel);
                    //    string errorMessage = "There is a difference in the number of tokens which we have sent to FireBase but the response contained different number of tokens. In this case we are not able to determine which token has a problem. This requires to be debuged.";
                    //    return $"{errorMessage}{Environment.NewLine}Tokens:{tokensAsString}{Environment.NewLine}FireBaseResponses:{responseModelAsString}";
                    //});

                    return SendTokensResult.Success;
                }

                var sendPushNotificationResult = new List<SubscriptionToken>();


                for (int i = 0; i < responseModel.Count; i++)
                {
                    var token = tokens[i];
                    if (string.Equals(responseModel[i].Error, FireBaseResponseResultModel.UnregisteredDevice, StringComparison.OrdinalIgnoreCase))
                    {
                        //log.Info($"[FireBase] the token: '{token}' is not registered and will be removed from the subscriber");
                        sendPushNotificationResult.Add(token);
                    }
                }

                return new SendTokensResult(sendPushNotificationResult);
            }
        }
    }
}
