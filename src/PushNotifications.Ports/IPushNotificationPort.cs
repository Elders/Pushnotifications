using PushSharp;
using PushSharp.Core;

namespace PushNotifications.Ports
{
    public interface IPushNotificationPort
    {
        IPushBroker PushBroker { get; set; }
    }
}
