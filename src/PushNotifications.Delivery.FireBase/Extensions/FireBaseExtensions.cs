using System;
using System.Linq;
using PushNotifications.Delivery.FireBase.Models;

namespace PushNotifications.Delivery.FireBase
{
    public static class FireBaseExtensions
    {
        public static bool HasDataFailure(this FireBaseResponseModel data)
        {
            return (data is null == false) && data.Failure > 0;
        }

        public static string GetDataErrors(this FireBaseResponseModel data)
        {
            if (data.HasDataFailure())
            {
                string dataErrors = $"DataErrors:{Environment.NewLine}" + string.Join(",", data.Results.Select(x => x.Error));
                return dataErrors;
            }

            return string.Empty;
        }

        //public static bool HasDataFailure(this IRestResponse<FireBaseResponseModel> response)
        //{
        //    return (response.Data is null == false) && response.Data.Failure == true;
        //}

        //public static string GetDataErrors(this IRestResponse<FireBaseResponseModel> response)
        //{
        //    if (response.HasDataFailure())
        //    {
        //        string dataErrors = $"DataErrors:{Environment.NewLine}" + string.Join(",", response.Data.Results.Select(x => x.Error));
        //        return dataErrors;
        //    }

        //    return string.Empty;
        //}

        //public static void LogFireBaseError(this IRestResponse<FireBaseResponseModel> response, Func<string> contextualMessage)
        //{
        //    if (response.ErrorException is null)
        //    {
        //        log.Error(() =>
        //        {
        //            string dataErrors = response.GetDataErrors();
        //            return $"{contextualMessage()}{Environment.NewLine}{dataErrors}{Environment.NewLine}{response.ErrorMessage}";
        //        });
        //    }
        //    else
        //    {
        //        string dataErrors = response.GetDataErrors();
        //        string errorMessage = $"{contextualMessage()}{Environment.NewLine}{dataErrors}{Environment.NewLine}{response.ErrorMessage}";
        //        log.ErrorException(errorMessage, response.ErrorException);
        //    }
        //}
    }
}
