using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Elders.Cronus;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public class NotificationForDelivery : ValueObject<NotificationForDelivery>
    {
        public NotificationForDelivery(NotificationPayload notificationPayload, Dictionary<string, object> notificationData, DateTimeOffset expiresAt, bool contentAvailable)
        {
           // if (ReferenceEquals(null, id) == true) throw new ArgumentNullException(nameof(id));
            if (ReferenceEquals(null, notificationPayload) == true) throw new ArgumentNullException(nameof(notificationPayload));
            if (ReferenceEquals(null, notificationData) == true) throw new ArgumentNullException(nameof(notificationData));
            if (ReferenceEquals(null, expiresAt) == true) throw new ArgumentNullException(nameof(expiresAt));

            //Id = id;
            NotificationPayload = notificationPayload;
            NotificationData = notificationData;
            ExpiresAt = expiresAt;
            ContentAvailable = contentAvailable;
        }

        public NotificationPayload NotificationPayload { get; private set; }

        public Dictionary<string, object> NotificationData { get; private set; }

        public DateTimeOffset ExpiresAt { get; private set; }

        public bool ContentAvailable { get; private set; }
    }

    [DataContract(Namespace = "pushnotifications", Name = "0b5bc529-e630-4b7a-a683-1377e270a417")]
    public class NotificationMessageSignal : ISignal
    {
        NotificationMessageSignal()
        {
            Recipients = new List<string>();
        }

        public NotificationMessageSignal(string recipient, NotificationPayload notificationPayload, Dictionary<string, object> notificationData, DateTimeOffset expiresAt, bool contentAvailable, string tenant)
            : this(new List<string>() { recipient }, notificationPayload, notificationData, expiresAt, contentAvailable, tenant)
        {

        }

        public NotificationMessageSignal(List<string> recipients, NotificationPayload notificationPayload, Dictionary<string, object> notificationData, DateTimeOffset expiresAt, bool contentAvailable, string tenant)
        {
            Recipients = recipients;
            NotificationPayload = notificationPayload;
            NotificationData = notificationData;
            ExpiresAt = expiresAt;
            ContentAvailable = contentAvailable;
            Tenant = tenant;
        }

        [DataMember(Order = 1)]
        public List<string> Recipients { get; private set; }

        [DataMember(Order = 2)]
        public NotificationPayload NotificationPayload { get; private set; }

        [DataMember(Order = 3)]
        public Dictionary<string, object> NotificationData { get; private set; }

        [DataMember(Order = 4)]
        public DateTimeOffset ExpiresAt { get; private set; }

        [DataMember(Order = 5)]
        public bool ContentAvailable { get; private set; }

        [DataMember(Order = 6)]
        public string Tenant { get; set; }

        public NotificationForDelivery ToDelivery()
        {
            return new NotificationForDelivery(NotificationPayload, NotificationData, ExpiresAt, ContentAvailable);
        }
    }
}
