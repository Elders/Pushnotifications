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
    }
}
