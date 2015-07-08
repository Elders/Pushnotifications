using Elders.Cronus.DomainModeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications.Events
{
    [DataContract(Name = "81f3d90b-8f53-4bfe-8260-6a59bf426dd9")]
    public class PushNotificationWasSent : IEvent
    {
        PushNotificationWasSent() { }

        public PushNotificationWasSent(PushNotificationId id, string userId, string json, string text, string sound, string icon, string category, int badge)
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
    }
}