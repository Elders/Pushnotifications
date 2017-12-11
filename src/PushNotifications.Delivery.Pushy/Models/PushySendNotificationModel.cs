namespace PushNotifications.Delivery.Pushy.Models
{
    public class PushySendNotificationModel
    {
        public PushySendNotificationModel(string title, string body, string sound, int badge)
        {
            Title = title;
            Body = body;
            Sound = sound;
            Badge = badge;
        }

        public string Title { get; private set; }

        public string Body { get; private set; }

        public string Sound { get; private set; }

        public int Badge { get; private set; }
    }
}
