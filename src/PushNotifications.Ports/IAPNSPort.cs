using Elders.Cronus.DomainModeling;
using PushNotifications.Contracts.PushNotifications.Events;
using PushSharp;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.Ports
{
    public interface IAPNSPort
    {
        PushBroker PushBroker { get; set; }
    }
}
