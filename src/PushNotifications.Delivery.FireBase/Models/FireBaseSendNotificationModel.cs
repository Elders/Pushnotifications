using System;

namespace PushNotifications.Delivery.FireBase
{
    public class FireBaseSendNotificationModel
    {
        public FireBaseSendNotificationModel(string title, string body, string sound, string badge)
        {
            if (string.IsNullOrEmpty(title) == true) throw new ArgumentNullException(nameof(title));
            if (string.IsNullOrEmpty(body) == true) throw new ArgumentNullException(nameof(body));

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
