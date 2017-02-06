using PushSharp.Core;

namespace PushNotifications.Throttling
{
    public interface IThrottleNotification
    {
        Notification ToNotification();
    }
}
