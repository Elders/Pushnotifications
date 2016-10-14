using System;
using PushSharp.Core;

namespace PushNotifications.Ports.Parse
{
    public class ParseAndroidNotifcation : Notification
    {
        public ParseAndroidNotifcation(string deviceToken, string json)
        {
            Json = json;
            Token = deviceToken;
            base.EnqueuedTimestamp = DateTime.UtcNow;
        }

        public override bool IsValidDeviceRegistrationId()
        {
            return true;
        }

        public string Token { get; private set; }

        public string Json { get; private set; }

        public NotificationObject BuildNotificationObject()
        {
            return new NotificationObject()
            {
                where = new NotificationObject.UserIdClause() { userId = Token },
                data = new NotificationObject.AndroidDataContract() { data = Json }
            };
        }

        public class NotificationObject
        {
            public object where { get; set; }

            public class UserIdClause
            {
                public string userId { get; set; }
            }

            public object data { get; set; }

            public class AndroidDataContract
            {
                public object data { get; set; }
            }
        }
    }
}