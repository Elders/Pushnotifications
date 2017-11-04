using System.Collections.Generic;

namespace PushNotifications.Delivery.FireBase.Models
{
    public class FireBaseResponseModel
    {
        public bool Success { get; set; }

        public bool Failure { get; set; }

        public List<FireBaseResponseResultModel> Results { get; set; }

        public class FireBaseResponseResultModel
        {
            public string Error { get; set; }
        }
    }
}
