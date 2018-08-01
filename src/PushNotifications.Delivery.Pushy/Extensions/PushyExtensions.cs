using System;
using PushNotifications.Delivery.Pushy.Logging;
using PushNotifications.Delivery.Pushy.Models;
using RestSharp;

namespace PushNotifications.Delivery.Pushy
{
    public static class PushyExtensions
    {
        static ILog log = LogProvider.GetLogger(typeof(PushyTopicSubscriptionManager));

        public static bool HasDataFailure(this IRestResponse<PushyResponseModel> response)
        {
            return (response.Data is null == false) && response.Data.Success == false;
        }

        public static string GetDataError(this IRestResponse<PushyResponseModel> response)
        {
            if (response.HasDataFailure())
            {
                return $"DataError: {response.Data.Error }";
            }

            return string.Empty;
        }

        public static void LogPushyError(this IRestResponse<PushyResponseModel> response, Func<string> contextualMessage)
        {
            if (response.ErrorException is null)
            {
                log.Error(() =>
                {
                    string dataError = response.GetDataError();
                    return $"{contextualMessage()}{Environment.NewLine}{dataError}{Environment.NewLine}{response.ErrorMessage}";
                });
            }
            else
            {
                string dataErrors = response.GetDataError();
                string errorMessage = $"{contextualMessage()}{Environment.NewLine}{dataErrors}{Environment.NewLine}{response.ErrorMessage}";
                log.ErrorException(errorMessage, response.ErrorException);
            }
        }
    }

}
