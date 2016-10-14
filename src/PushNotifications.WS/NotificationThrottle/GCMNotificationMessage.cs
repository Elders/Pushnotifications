using System.Linq;
using System.Runtime.Serialization;
using PushSharp.Android;
using PushSharp.Core;
using PushSharp;

namespace PushNotifications.WS.NotificationThrottle
{
    [DataContract(Name = "6f9779ab-ddac-41b1-9deb-01826d1429da")]
    public class GCMNotificationMessage : IThrottleNotification
    {
        GCMNotificationMessage() { }

        public GCMNotificationMessage(GcmNotification notification)
        {
            Token = notification.RegistrationIds.Single();
            Json = notification.JsonData;
        }

        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public string Json { get; set; }

        public Notification ToNotification()
        {
            return new GcmNotification()
            .ForDeviceRegistrationId(Token)
            .WithJson(Json);
        }
    }
}
