using Elders.Cronus.DomainModeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications.Commands
{
    [DataContract(Name = "4a3349c4-702e-4c6e-b6c2-f3276d729b42")]
    public class SendPushNotification : ICommand
    {
        SendPushNotification() { }

        public SendPushNotification(PushNotificationId id, string userId, string json, string text, string sound, string icon, string category, int badge)
        {
            Id = id;
            UserId = userId;
            Json = json;
            Text = text;
            Sound = sound;
            Icon = icon;
            Category = category;
            Badge = badge;
        }

        [DataMember(Order = 1)]
        public PushNotificationId Id { get; private set; }

        [DataMember(Order = 2)]
        public string UserId { get; private set; }

        [DataMember(Order = 3)]
        public string Json { get; private set; }

        [DataMember(Order = 4)]
        public string Text { get; private set; }

        [DataMember(Order = 5)]
        public string Sound { get; private set; }

        [DataMember(Order = 6)]
        public string Icon { get; private set; }

        [DataMember(Order = 7)]
        public string Category { get; private set; }

        [DataMember(Order = 8)]
        public int Badge { get; private set; }

        public bool IsValid()
        {
            return
                ReferenceEquals(null, Id) == false &&
                !string.IsNullOrWhiteSpace(UserId);
        }
    }
}