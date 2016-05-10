using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp;

namespace PushNotifications.WS.NotificationThrottle
{
    [DataContract(Name = "a9602d77-6290-4306-8693-475a2109dfe1")]
    public class APNSNotificationMessage : IThrottleNotification
    {
        APNSNotificationMessage() { }

        public APNSNotificationMessage(AppleNotification notification)
        {
            Token = notification.DeviceToken;
            CustomItems = notification.Payload.CustomItems.ToDictionary(x => x.Key, x => x.Value.Single().ToString());
            Sound = notification.Payload.Sound;
            Category = notification.Payload.Category;
            Body = notification.Payload.Alert.Body;
            if (notification.Payload.Badge.HasValue)
                Badge = notification.Payload.Badge.Value;
        }

        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public string Category { get; set; }

        [DataMember(Order = 3)]
        public string Body { get; set; }

        [DataMember(Order = 4)]
        public Dictionary<string, string> CustomItems { get; set; }

        [DataMember(Order = 5)]
        public string Sound { get; set; }

        [DataMember(Order = 6)]
        public int Badge { get; set; }

        public Notification ToNotification()
        {
            var notification = new AppleNotification()
                .ForDeviceToken(Token)
                .WithContentAvailable(1)
                .WithAlert(Body)
                .WithSound(Sound)
                .WithBadge(Badge)
                .WithCategory(Category);

            foreach (var item in CustomItems)
            {
                notification.WithCustomItem(item.Key, item.Value);
            }

            return notification;
        }
    }
}
