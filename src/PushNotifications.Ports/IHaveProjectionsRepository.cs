using Elders.Cronus.DomainModeling.Projections;

namespace PushNotifications.Ports
{
    public interface IHaveProjectionsRepository
    {
        IRepository Repository { get; set; }
    }
}
