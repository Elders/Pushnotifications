using Elders.Cronus.DomainModeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace PushNotifications.Contracts.PushNotifications
{
    [DataContract(Name = "2ec66371-ccff-48da-86b7-5244caa6e073")]
    public class PushNotificationId : GuidId
    {
        protected PushNotificationId() { }
        public PushNotificationId(Guid id) : base(id, "PushNotification") { }
    }
}