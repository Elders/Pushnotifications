using System.Text.Json.Serialization;

namespace PushNotifications.Delivery.FireBase
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

        [JsonPropertyName("title")]
        public string Title { get; private set; }

        [JsonPropertyName("body")]
        public string Body { get; private set; }

        [JsonPropertyName("sound")]
        public string Sound { get; private set; }

        [JsonPropertyName("badge")]
        public string Badge { get; private set; }
    }
}
