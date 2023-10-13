using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Elders.Cronus;

namespace PushNotifications.Contracts.PushNotifications.Delivery
{
    public class NotificationTarget
    {
        public NotificationTarget(string tenant, string application)
        {
            Tenant = tenant;
            Application = application;
        }

        public string Tenant { get; private set; }

        public string Application { get; private set; }
    }

    public class NotificationForDelivery : ValueObject<NotificationForDelivery>
    {
        public NotificationForDelivery(NotificationPayload notificationPayload, Dictionary<string, object> notificationData, DateTimeOffset expiresAt, bool contentAvailable, NotificationTarget target)
        {
            // if (ReferenceEquals(null, id) == true) throw new ArgumentNullException(nameof(id));
            if (notificationPayload is null == true) throw new ArgumentNullException(nameof(notificationPayload));
            if (notificationData is null == true) throw new ArgumentNullException(nameof(notificationData));
            // if (ReferenceEquals(null, expiresAt) == true) throw new ArgumentNullException(nameof(expiresAt));

            //Id = id;
            NotificationPayload = notificationPayload;
            NotificationData = notificationData;
            ExpiresAt = expiresAt;
            ContentAvailable = contentAvailable;
            Target = target;
        }

        public NotificationPayload NotificationPayload { get; private set; }

        public Dictionary<string, object> NotificationData { get; private set; }

        public DateTimeOffset ExpiresAt { get; private set; }

        public bool ContentAvailable { get; private set; }

        public NotificationTarget Target { get; private set; }
    }

    [DataContract(Namespace = "pushnotifications", Name = "0b5bc529-e630-4b7a-a683-1377e270a417")]
    public class NotificationMessageSignal : ISignal
    {
        NotificationMessageSignal()
        {
            Recipients = new List<string>();
        }

        public NotificationMessageSignal(string recipient, NotificationPayload notificationPayload, Dictionary<string, object> notificationData, DateTimeOffset expiresAt, bool contentAvailable, NotificationTarget target)
            : this(new List<string>() { recipient }, notificationPayload, notificationData, expiresAt, contentAvailable, target)
        {

        }

        public NotificationMessageSignal(List<string> recipients, NotificationPayload notificationPayload, Dictionary<string, object> notificationData, DateTimeOffset expiresAt, bool contentAvailable, NotificationTarget target)
        {
            Recipients = recipients;
            NotificationPayload = notificationPayload;
            NotificationData = notificationData;
            ExpiresAt = expiresAt;
            ContentAvailable = contentAvailable;
            Tenant = target.Tenant;
            Application = target.Application;
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
        public string Tenant { get; private set; }

        [DataMember(Order = 7)]
        public string Application { get; private set; }

        public NotificationForDelivery ToDelivery()
        {
            return new NotificationForDelivery(NotificationPayload, NotificationData, ExpiresAt, ContentAvailable, new NotificationTarget(Tenant, Application));
        }
    }
}
