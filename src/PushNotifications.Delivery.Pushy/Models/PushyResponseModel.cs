using PushNotifications.Delivery.Pushy.Logging;

namespace PushNotifications.Delivery.Pushy.Models
{
    public class PushyResponseModel
    {
        static ILog log = LogProvider.GetLogger(typeof(PushyResponseModel));

        public bool Success { get; set; }

        public string Error { get; set; }
    }
}
