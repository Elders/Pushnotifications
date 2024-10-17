namespace PushNotifications.Delivery.Pushy.Models
{
    public class PushyResponseModel
    {
        public bool Success { get; set; }

        public PushyErrorModel Error { get; set; }
    }

    public class PushyErrorModel
    {
        public string Code { get; set; }

        public string Error { get; set; }
    }
}
