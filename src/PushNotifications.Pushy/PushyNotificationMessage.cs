using PushSharp.Core;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections.Generic;
using PushNotifications.Throttling;

namespace PushNotifications.Pushy
{
    [DataContract(Name = "8e70ab97-240d-4919-a48a-9f53a85428a2")]
    public class PushyNotificationMessage : IThrottleNotification
    {
        public PushyNotificationMessage(PushyNotification notification)
        {
            Token = notification.Tokens.Single();
            Json = notification.JsonnData;
        }

        [DataMember(Order = 1)]
        public string Token { get; private set; }

        [DataMember(Order = 2)]
        public string Json { get; private set; }

        public Notification ToNotification()
        {
            return new PushyNotification(Json, new List<string> { Token });
        }
    }
}
