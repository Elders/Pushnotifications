using PushSharp;

namespace PushNotifications.Ports
{
    public interface IPushNotificationPort
    {
        PushBroker PushBroker { get; set; }
    }
}
