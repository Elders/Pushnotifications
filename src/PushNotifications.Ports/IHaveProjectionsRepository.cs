using Elders.Cronus.DomainModeling.Projections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.Ports
{
    public interface IHaveProjectionsRepository
    {
        IRepository Repository { get; set; }
    }
}
