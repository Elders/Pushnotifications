using Elders.Cronus.DomainModeling;
using System.Runtime.Serialization;
using System;

namespace PushNotifications.Contracts.PushNotifications
{
    [DataContract(Name = "dd74d54e-a72c-4506-b2a4-3f9d20c922bc")]
    public class NotificationPayload : ValueObject<NotificationPayload>
    {
        NotificationPayload() { }

        public NotificationPayload(string title, string body) : this(title, body, string.Empty, string.Empty, 0)
        { }

        public NotificationPayload(string title, string body, string sound, string icon, int badge)
        {
            Title = title;
            Body = body;
            Sound = sound;
            Icon = icon;
            Badge = badge;
        }

        [DataMember(Order = 1)]
        public string Title { get; private set; }

        [DataMember(Order = 2)]
        public string Body { get; private set; }

        [DataMember(Order = 3)]
        public string Sound { get; private set; }

        [DataMember(Order = 4)]
        public string Icon { get; private set; }

        [DataMember(Order = 5)]
        public int Badge { get; private set; }

        public override string ToString()
        {
            return $"{nameof(Title)}: '{Title}', {nameof(Body)}: '{Body}', {nameof(Sound)}: '{Sound}', {nameof(Icon)}: '{Icon}', {nameof(Badge)}: '{Badge}'";
        }
    }
}
