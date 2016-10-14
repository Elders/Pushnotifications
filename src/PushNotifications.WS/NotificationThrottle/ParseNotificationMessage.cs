using System.Runtime.Serialization;
using PushNotifications.Ports.Parse;
using PushSharp.Core;

namespace PushNotifications.WS.NotificationThrottle
{
    [DataContract(Name = "27aeed29-4044-4396-8a09-724662a96c03")]
    public class ParseNotificationMessage : IThrottleNotification
    {
        ParseNotificationMessage() { }

        public ParseNotificationMessage(ParseAndroidNotifcation notification)
        {
            Token = notification.Token;
            Json = notification.Json;
        }

        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public string Json { get; set; }

        public Notification ToNotification()
        {
            return new ParseAndroidNotifcation(Token, Json);
        }
    }
}
