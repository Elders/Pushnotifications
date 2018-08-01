using System;
using System.Linq;
using PushNotifications.Delivery.FireBase.Logging;
using PushNotifications.Delivery.FireBase.Models;
using RestSharp;

namespace PushNotifications.Delivery.FireBase
{
    public static class FireBaseExtensions
    {
        static ILog log = LogProvider.GetLogger(typeof(FireBaseTopicSubscriptionManager));

        public static bool HasDataFailure(this IRestResponse<FireBaseResponseModel> response)
        {
            return (response.Data is null == false) && response.Data.Failure == true;
        }

        public static string GetDataErrors(this IRestResponse<FireBaseResponseModel> response)
        {
            if (response.HasDataFailure())
            {
                string dataErrors = $"DataErrors:{Environment.NewLine}" + string.Join(",", response.Data.Results.Select(x => x.Error));
                return dataErrors;
            }

            return string.Empty;
        }

        public static void LogFireBaseError(this IRestResponse<FireBaseResponseModel> response, Func<string> contextualMessage)
        {
            if (response.ErrorException is null)
            {
                log.Error(() =>
                {
                    string dataErrors = response.GetDataErrors();
                    return $"{contextualMessage()}{Environment.NewLine}{dataErrors}{Environment.NewLine}{response.ErrorMessage}";
                });
            }
            else
            {
                string dataErrors = response.GetDataErrors();
                string errorMessage = $"{contextualMessage()}{Environment.NewLine}{dataErrors}{Environment.NewLine}{response.ErrorMessage}";
                log.ErrorException(errorMessage, response.ErrorException);
            }
        }
    }
}
