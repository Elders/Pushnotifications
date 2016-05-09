using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp;

namespace PushNotifications.WS.NotificationThrottle
{
    public interface IThrottleNotification
    {
        Notification ToNotification();
    }
}