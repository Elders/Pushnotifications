namespace PushNotifications.Delivery.FireBase.Models
{
    public class FireBaseSendNotificationModel
    {
        public FireBaseSendNotificationModel(string title, string body, string sound, string badge)
        {
            Title = title;
            Body = body;
            Sound = sound;
            Badge = badge;
        }

        public string Title { get; private set; }

        public string Body { get; private set; }

        public string Sound { get; private set; }

        public string Badge { get; private set; }
    }
}
