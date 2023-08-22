using System.Text.Json.Serialization;

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

        [JsonPropertyName("title")]
        public string Title { get; private set; }

        [JsonPropertyName("body")]
        public string Body { get; private set; }

        [JsonPropertyName("sound")]
        public string Sound { get; private set; }

        [JsonPropertyName("badge")]
        public int Badge { get; private set; }
    }
}
